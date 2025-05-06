# Ray Tracer Built From Scratch

A comprehensive ray tracing engine implemented in C#, capable of rendering photorealistic 3D scenes with advanced lighting and material effects.

## Features Overview

### Core Rendering Engine

- Recursive Ray Tracing: Support for ray bouncing to create realistic reflections and refractions
- Progressive Rendering: Line-by-line rendering with progress tracking
- Configurable Recursion Depth: Control maximum ray bounce depth for performance tuning
- Multi-sampling: Support for multiple samples per pixel for anti-aliasing

### Acceleration Structures

- Bounding Volume Hierarchy (BVH): Spatial acceleration structure for efficient ray intersection tests
- Axis-Aligned Bounding Boxes (AABB): Fast collision detection for ray-box intersection
- Spatial Partitioning: Optimized scene traversal using axis splitting and recursive subdivision

### Geometric Primitives

- **Built-in Shapes**:
  - Spheres with material properties
  - Infinite planes with configurable position and normal
  - Triangles with vertex positions and normals
  - Rectangles with customizable dimensions and orientation
  - Cubes with rotation support

- **3D Model Import**:
  - OBJ file format support with triangulation
  - MTL material loading
  - Normal handling for proper lighting

### Lighting System

- **Multiple Light Types**:
  - Ambient lighting with configurable intensity
  - Diffused lights with position, color, and attenuation
  
- **Advanced Lighting Effects**:
  - Physically-based attenuation (inverse square law)
  - Soft shadows through transparent objects
  - Shadow color tinting through colored transparent objects

### Material System

- **Physically-Based Materials**:
  - Diffuse, specular, ambient, and emissive color components
  - Configurable shininess for controlling specular highlights
  - Support for reflective surfaces with variable reflectivity
  - Transparent materials with configurable opacity
  - Refraction with customizable indices of refraction

- **Fresnel Effects**:
  - Schlick's approximation for realistic angle-dependent reflectivity
  - Total internal reflection calculation
  - Proper handling of entering and exiting transparent objects

### Camera System

- **Flexible Camera Controls**:
  - Configurable position, lookAt point, and field of view
  - Support for different aspect ratios
  - Up vector specification for camera orientation
  - Ray generation for each pixel with optional jitter for anti-aliasing

- **Animation Support**:
  - Camera tracking shots along predefined paths
  - Automated frame generation for animations

### Output Options

- **Image Generation**:
  - Bitmap output using SixLabors.ImageSharp
  - Configurable image resolution
  - Support for creating animated GIFs from render sequences

## Technical Implementation Details

### Ray-Object Intersection

The ray tracer implements mathematically accurate intersection algorithms for:

- Sphere-ray intersection using quadratic equation solving
- Triangle-ray intersection using matrix operations
- AABB-ray intersection for efficient BVH traversal
- Plane-ray intersection with normal-based calculations

### BVH Construction and Traversal

- Split axis selection based on object extent
- Recursive BVH node building with configurable leaf size
- Efficient BVH traversal prioritizing closer intersections
- Spatial sorting of objects for optimal BVH performance

### Lighting and Shading

- Direct illumination calculation with diffuse and specular components
- Shadow ray casting with transparent object handling
- Multiple light contribution accumulation
- Ambient light for base illumination

### Reflection and Refraction

- Accurate reflection vector calculation
- Snell's law implementation for refraction
- Fresnel effect simulation for realistic material transitions
- Recursive ray tracing with configurable depth limit
