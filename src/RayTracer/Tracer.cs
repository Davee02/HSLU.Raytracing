using Common;
using System.Numerics;

namespace RayTracer;
internal static class Tracer
{
    internal static Color TraceRay(Ray ray, Scene scene, int depth, int maxDepth)
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
        var contributingLights = new List<(Light light, float diffuseFactor, float specularFactor)>();
        foreach (var light in scene.DiffusedLights)
        {
            // Shadow ray from intersection to this light
            var lightDirection = Vector3.Normalize(light.Position - hit.Position);
            var shadowRay = new Ray(hit.Position, lightDirection);

            var closestShadowHit = FindClosestObject(shadowRay, scene);
            var lightDistance = (light.Position - hit.Position).Length();

            if (!closestShadowHit.HasValue || closestShadowHit.Value.Lambda > lightDistance)
            {
                // No shadow for this light, add its contribution
                var diffuseFactor = Vector3.Dot(lightDirection, hit.Normal);

                if (diffuseFactor > 0)
                {
                    var specularFactor = Vector3.Dot(reflectionRay.Direction, shadowRay.Direction);
                    specularFactor = MathF.Max(0, specularFactor);
                    specularFactor = MathF.Pow(specularFactor, hit.Material.Shininess);

                    contributingLights.Add((light, diffuseFactor, specularFactor));
                }
            }
        }

        if (contributingLights.Count > 0)
        {
            // Apply normalization factor
            var lightFactor = 1f / contributingLights.Count; // normalize by dividing by the number of contributing light sources
            foreach (var (light, diffuseFactor, specularFactor) in contributingLights)
            {
                var diffuseContribution = hit.Material.Color * diffuseFactor;
                var specularContribution = light.Color * specularFactor;

                pixelColor += light.Color * light.Intensity * (diffuseContribution + specularContribution);
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

    // No helper methods needed with simplified approach

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