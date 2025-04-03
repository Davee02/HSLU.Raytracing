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

        // Calculate reflection color (if material is reflective)
        if (hit.Material.Reflectivity > 0)
        {
            var reflectionColor = TraceRay(reflectionRay, scene, depth + 1, maxDepth);
            // Mix the reflection color with the direct illumination color based on reflectivity
            pixelColor = pixelColor * (1 - hit.Material.Reflectivity) + reflectionColor * hit.Material.Reflectivity;
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
