# SpriteAfterimage

High-performance afterimage effect for Unity 2D using GPU instancing

[![GitHub license](https://img.shields.io/github/license/nuskey8/SpriteAfterimage)](LICENSE)
[![Unity](https://img.shields.io/badge/Unity-6000.5%2B-black?logo=unity&logoColor=white)]()

English | [日本語](README.ja.md)

![demo](docs/demo.gif)

SpriteAfterimage is a library that provides a component for rendering afterimages of SpriteRenderer in Unity.

By utilizing Unity 6.5's `Graphics.RenderSpriteInstanced`, it achieves high-performance afterimage rendering through GPU instancing, drawing them all together without generating a GameObject or SpriteRenderer for each afterimage.

## Setup

### Requirements

- Unity 6.5 or later
- Universal Render Pipeline (URP)

### Installation

1. Open Package Manager from Window > Package Manager
2. Click the "+" button > Add package from git URL
3. Enter the following URL:

```
https://github.com/nuskey8/SpriteAfterimage.git?path=Assets/SpriteAfterimage
```

Alternatively, open Packages/manifest.json and add the following to the dependencies block:

```json
{
    "dependencies": {
        "com.nuskey8.spriteafterimage": "https://github.com/nuskey8/SpriteAfterimage.git?path=Assets/SpriteAfterimage"
    }
}
```

## Quick Start

1. Add the `SpriteAfterimage` component to the GameObject on which you want to display afterimages.
2. Specify the target `SpriteRenderer` in `Source`. If added to the same GameObject, it will be set automatically when the component is added.
3. Specify the appropriate shader for your use case in `Shader`.
   - `SpriteAfterimage/Unlit`: Afterimages unaffected by 2D Light
   - `SpriteAfterimage/Lit`: Afterimages affected by URP 2D Light
4. Adjust `Emit Interval`, `Lifetime`, `Color`, and other settings.

## Configuration Properties

![inspector](docs/inspector.png)

| Property               | Description                                                                                        |
| ---------------------- | -------------------------------------------------------------------------------------------------- |
| `Source`               | The SpriteRenderer used as the source for recording afterimages                                    |
| `Emission Enabled`     | Whether to emit new afterimages                                                                    |
| `Emit Interval`        | The interval (in seconds) at which afterimages are recorded                                        |
| `Lifetime`             | The duration (in seconds) that each afterimage is displayed                                        |
| `Color`                | The color of the afterimage                                                                        |
| `Color Mode`           | `Tint`: Multiplies `Color` with the original sprite color <br> `Solid`: Fills with a solid `Color` |
| `Fade`                 | The alpha value of the afterimage relative to its elapsed time                                     |
| `Use Unscaled Time`    | When enabled, it is not affected by `Time.timeScale`                                               |
| `Shader`               | The shader used to render afterimages                                                              |
| `Sorting Order Offset` | The value added to the Sorting Order of the original SpriteRenderer                                |

## GPU Instancing

By default, `SpriteAfterimage` renders afterimages of the same sprite using `Graphics.RenderSpriteInstanced`; however, if the runtime environment does not support GPU instancing, it renders them individually using `Graphics.RenderSprite`. If performance becomes an issue in such environments, it is recommended to limit the number of afterimages.

## License

[MIT](LICENSE)

The Unity-chan asset used in the demo is provided under the [Unity-chan License Terms](https://unity3d.jp/unity-chan/license?lang=en).

![unitychan-logo](Assets/UnityChan/UCL3.0/License%20Logo/Others/png/Light_Frame.png)