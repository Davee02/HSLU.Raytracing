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

        // Start with ambient light
        var pixelColor = hit.Material.AmbientColor * scene.AmbientLight.AttenuationC;

        // Add material's emissive component directly
        //pixelColor += hit.Material.EmissiveColor;

        // Accumulate contributions from all diffused lights
        var contributingLights = new List<(Light light, float diffuseFactor, float specularFactor, float shadowFactor, Color shadowColor, float attenuationFactor)>();
        foreach (var light in scene.DiffusedLights)
        {
            // Shadow ray from intersection to this light
            var lightDirection = Vector3.Normalize(light.Position - hit.Position);
            var shadowRay = new Ray(hit.Position, lightDirection);

            var lightDistance = (light.Position - hit.Position).Length();

            // Calculate distance attenuation using the formula: 1/(a*d^2 + b*d + c)
            float attenuationFactor = 1.0f / (
                (light.AttenuationA * lightDistance * lightDistance) +
                (light.AttenuationB * lightDistance) +
                light.AttenuationC
            );

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
                    shadowColor = new Color(
                        shadowColor.R * (1 - tintStrength + (shadowHit.Value.Material.DiffuseColor.R * tintStrength)),
                        shadowColor.G * (1 - tintStrength + (shadowHit.Value.Material.DiffuseColor.G * tintStrength)),
                        shadowColor.B * (1 - tintStrength + (shadowHit.Value.Material.DiffuseColor.B * tintStrength))
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
                        shadowHit.Value.Position + (currentRay.Direction * ITraceableObject.eps * 10),
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

                    contributingLights.Add((light, diffuseFactor, specularFactor, shadowFactor, shadowColor, attenuationFactor));
                }
            }
        }

        if (contributingLights.Count > 0)
        {
            foreach (var (light, diffuseFactor, specularFactor, shadowFactor, shadowColor, attenuationFactor) in contributingLights)
            {
                // Use the material's diffuse color for diffuse component
                var diffuseContribution = hit.Material.DiffuseColor * diffuseFactor;

                // Use the material's specular color for specular highlights
                var specularContribution = hit.Material.SpecularColor * light.Color * specularFactor;

                // Apply shadow factor, shadow color, and distance attenuation to the light contribution
                pixelColor += light.Color * shadowColor * shadowFactor * attenuationFactor * (diffuseContribution + specularContribution);
            }
        }

        // Store the direct illumination color before applying reflection/refraction
        var directColor = pixelColor;

        // Calculate reflection and refraction
        Color reflectionColor = Color.Black;
        Color refractionColor = Color.Black;

        // Determine if we're entering or exiting the material
        bool entering = Vector3.Dot(ray.Direction, hit.Normal) < 0;
        Vector3 normal = entering ? hit.Normal : -hit.Normal;

        // Set refractive indices based on whether we're entering or exiting
        float n1, n2; // Refractive indices
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

        // Calculate the cosine of the angle between ray direction and normal
        float cosTheta = Math.Abs(Vector3.Dot(-ray.Direction, normal));

        // Calculate Schlick's approximation for Fresnel factor
        float r0 = (n1 - n2) / (n1 + n2);
        r0 = r0 * r0;
        float fresnel = r0 + ((1 - r0) * MathF.Pow(1 - cosTheta, 5));

        // Scale the Fresnel effect by the material's reflectivity
        float effectiveReflectivity = hit.Material.Reflectivity * fresnel;

        // If material is transparent, calculate refraction
        float sinT2 = 0;
        bool totalInternalReflection = false;

        if (hit.Material.Transparency > 0)
        {
            float eta = n1 / n2;
            float cosI = Vector3.Dot(-ray.Direction, normal);
            sinT2 = eta * eta * (1.0f - (cosI * cosI));

            // Check for total internal reflection
            if (sinT2 >= 1.0f)
            {
                totalInternalReflection = true;
            }
        }

        // Calculate reflection color if material reflects or has total internal reflection
        if (effectiveReflectivity > 0 || totalInternalReflection)
        {
            reflectionColor = TraceRay(reflectionRay, scene, depth + 1, maxDepth);
        }

        // Calculate refraction color (if material is transparent and not total internal reflection)
        if (hit.Material.Transparency > 0 && !totalInternalReflection)
        {
            float eta = n1 / n2;
            float cosI = Vector3.Dot(-ray.Direction, normal);
            float cosT = MathF.Sqrt(1.0f - sinT2);
            Vector3 refractionDirection = Vector3.Normalize((eta * ray.Direction) + (((eta * cosI) - cosT) * normal));

            // Create refraction ray with slight offset to avoid self-intersection
            var refractionRay = new Ray(hit.Position + (refractionDirection * 5 * ITraceableObject.eps), refractionDirection);
            refractionColor = TraceRay(refractionRay, scene, depth + 1, maxDepth);
        }

        // Combine contributions
        if (totalInternalReflection)
        {
            // Total internal reflection - 100% reflection
            pixelColor = reflectionColor;
        }
        else
        {
            // Apply material reflectivity first (base reflection)
            if (effectiveReflectivity > 0)
            {
                pixelColor = (directColor * (1 - effectiveReflectivity)) + (reflectionColor * effectiveReflectivity);
            }

            // Then apply transparency if the material is transparent
            if (hit.Material.Transparency > 0)
            {
                // Apply Fresnel factor to adjust transparency at grazing angles
                float effectiveTransparency = hit.Material.Transparency * (1 - fresnel);
                pixelColor = (pixelColor * (1 - effectiveTransparency)) + (refractionColor * effectiveTransparency);
            }
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