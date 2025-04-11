using System.Numerics;

namespace Common;
public static class Tracer
{
    public static Color TraceRay(Ray ray, Scene scene, int depth, int maxDepth)
    {
        // Base case: if we've reached max recursion depth, return background color
        if (depth > maxDepth)
        {
            return scene.BackgroundColor;
        }

        var closestHit = FindClosestObject(ray, scene);
        if (!closestHit.HasValue)
        {
            return scene.BackgroundColor;
        }

        var hit = closestHit.Value;

        // Calculate reflection ray
        var reflectionRay = new Ray(hit.Position, Vector3.Normalize(Vector3.Reflect(ray.Direction, hit.Normal)));

        // Start with ambient light only
        var pixelColor = scene.ComputeAmbientColor(hit.Material);

        // Accumulate contributions from all diffused lights
        var contributingLights = new List<(Light light, float diffuseFactor, float specularFactor, float shadowFactor, Color shadowColor)>();
        foreach (var light in scene.DiffusedLights)
        {
            // Shadow ray from intersection to this light
            var lightDirection = Vector3.Normalize(light.Position - hit.Position);
            var shadowRay = new Ray(hit.Position, lightDirection);

            var lightDistance = (light.Position - hit.Position).Length();

            // Calculate shadow attenuation by tracing through transparent objects
            float shadowFactor = 1.0f; // Start with full light (no shadow)
            Color shadowColor = Color.White; // Start with no tint

            // We'll trace the shadow ray through potentially multiple objects
            var currentRay = shadowRay;
            float distanceCovered = 0;

            // Limit the number of transparent objects we'll process to avoid infinite loops
            for (int shadowDepth = 0; shadowDepth < maxDepth; shadowDepth++)
            {
                var shadowHit = FindClosestObject(currentRay, scene);

                // If we hit nothing or we've gone beyond the light, we're done
                if (!shadowHit.HasValue || distanceCovered + shadowHit.Value.Lambda > lightDistance)
                {
                    break;
                }

                // Accumulate the distance we've traveled
                distanceCovered += shadowHit.Value.Lambda;

                // Check if this object is transparent
                if (shadowHit.Value.Material.Transparency > 0)
                {
                    // Reduce light by the object's opacity (1 - transparency)
                    shadowFactor *= shadowHit.Value.Material.Transparency;

                    // Calculate the amount of opacity-based tinting
                    float tintStrength = 1 - shadowHit.Value.Material.Transparency;

                    // Tint the shadow with the object's color based on opacity
                    // For a fully transparent object (transparency=1), shadowColor remains unchanged
                    // For a partially transparent object, shadowColor gets partially tinted
                    shadowColor = new Color(
                        shadowColor.R * (1 - tintStrength + shadowHit.Value.Material.Color.R * tintStrength),
                        shadowColor.G * (1 - tintStrength + shadowHit.Value.Material.Color.G * tintStrength),
                        shadowColor.B * (1 - tintStrength + shadowHit.Value.Material.Color.B * tintStrength)
                    );

                    // If almost no light is left, consider it completely in shadow
                    if (shadowFactor < 0.01f)
                    {
                        shadowFactor = 0;
                        shadowColor = Color.Black;
                        break;
                    }

                    // Continue the ray from the exit point
                    currentRay = new Ray(
                        shadowHit.Value.Position + currentRay.Direction * ITraceableObject.eps * 10,
                        currentRay.Direction
                    );
                }
                else
                {
                    // If we hit an opaque object, we're in full shadow
                    shadowFactor = 0;
                    shadowColor = Color.Black;
                    break;
                }
            }

            // If we have any light contribution
            if (shadowFactor > 0)
            {
                var diffuseFactor = Vector3.Dot(lightDirection, hit.Normal);

                if (diffuseFactor > 0)
                {
                    var specularFactor = Vector3.Dot(reflectionRay.Direction, shadowRay.Direction);
                    specularFactor = MathF.Max(0, specularFactor);
                    specularFactor = MathF.Pow(specularFactor, hit.Material.Shininess);

                    contributingLights.Add((light, diffuseFactor, specularFactor, shadowFactor, shadowColor));
                }
            }
        }

        if (contributingLights.Count > 0)
        {
            // Apply normalization factor
            var lightFactor = 1f / contributingLights.Count;
            foreach (var (light, diffuseFactor, specularFactor, shadowFactor, shadowColor) in contributingLights)
            {
                var diffuseContribution = hit.Material.Color * diffuseFactor;
                var specularContribution = light.Color * specularFactor;

                // Apply both shadow factor and shadow color to the light contribution
                pixelColor += light.Color * shadowColor * light.Intensity * shadowFactor * (diffuseContribution + specularContribution);
            }
        }

        // Store the direct illumination color before applying reflection/refraction
        var directColor = pixelColor;

        // Calculate reflection and refraction
        Color reflectionColor = Color.Black;
        Color refractionColor = Color.Black;

        // Calculate reflection color (if material is reflective)
        if (hit.Material.Reflectivity > 0)
        {
            reflectionColor = TraceRay(reflectionRay, scene, depth + 1, maxDepth);
        }

        // Calculate refraction color (if material is transparent)
        if (hit.Material.Transparency > 0)
        {
            // Calculate refraction direction using Snell's law
            Vector3 normal = hit.Normal;
            float n1, n2; // Refractive indices

            // Determine if we're entering or exiting the material
            bool entering = Vector3.Dot(ray.Direction, normal) < 0;
            if (!entering)
            {
                normal = -normal; // Flip normal if we're exiting
            }

            // Set refractive indices based on whether we're entering or exiting
            if (entering)
            {
                n1 = 1.0f; // From air (or vacuum)
                n2 = hit.Material.RefractionIndex; // To material
            }
            else
            {
                n1 = hit.Material.RefractionIndex; // From material
                n2 = 1.0f; // To air (or vacuum)
            }

            float eta = n1 / n2;
            float cosI = Math.Abs(Vector3.Dot(ray.Direction, normal));
            float sinT2 = eta * eta * (1.0f - cosI * cosI);

            // Handle refraction or total internal reflection
            if (sinT2 < 1.0f)
            {
                // Refraction is possible
                float cosT = MathF.Sqrt(1.0f - sinT2);
                Vector3 refractionDirection = Vector3.Normalize(eta * ray.Direction + (eta * cosI - cosT) * normal);

                // Create refraction ray with slight offset to avoid self-intersection
                var refractionRay = new Ray(hit.Position + refractionDirection * 5*ITraceableObject.eps, refractionDirection);
                refractionColor = TraceRay(refractionRay, scene, depth + 1, maxDepth);
            }
            else
            {
                // Total internal reflection - all light is reflected
                refractionColor = reflectionColor;
            }
        }

        // Combine the contributions from direct lighting, reflection, and refraction

        // Handle reflection and refraction with simple linear blending
        if (hit.Material.Reflectivity > 0)
        {
            // Apply reflection based on reflectivity
            pixelColor = directColor * (1 - hit.Material.Reflectivity) +
                        reflectionColor * hit.Material.Reflectivity;
        }

        if (hit.Material.Transparency > 0)
        {
            // Apply refraction based on transparency
            pixelColor = pixelColor * (1 - hit.Material.Transparency) +
                        refractionColor * hit.Material.Transparency;
        }

        return pixelColor;
    }

    internal static Hit? FindClosestObject(Ray ray, Scene scene)
    {
        Hit hit = default;
        Hit? closestHit = default;
        float closestLambda = float.MaxValue;

        foreach (var obj in scene.TraceableObjects)
        {
            if (obj.TryIntersect(ray, ref hit) && hit.Lambda < closestLambda)
            {
                closestHit = hit;
                closestLambda = hit.Lambda;
            }
        }

        return closestHit;
    }
}