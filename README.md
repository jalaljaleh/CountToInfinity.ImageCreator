<div id="top"></div>

<!-- PROJECT LOGO -->
<br />
<div align="center">
  <a href="#">
    <img src="https://raw.githubusercontent.com/jalaljaleh/CountToInfinity.ImageCreator/refs/heads/master/CountToInfinity/countingjourney.ico" alt="Logo" width="150" height="150">
  </a>
  <h3 align="center"> Count To Infinity Image Generator</h3>
  <p align="center">
  Count To Infinity Image Generator, generates image files for sequential numbers by compositing digit sprites onto a template
    <br />  <br />
    <a href="https://www.instagram.com/countingjourney/">Instagram page</a>
    ·
    <a href="https://github.com/JalalJaleh/CountToInfinity.ImageCreator/releases">Releases</a>
    ·
    <a href="https://github.com/JalalJaleh/CountToInfinity.ImageCreator/issues">Report Bug</a>
    ·
    <a href="https://github.com/JalalJaleh/CountToInfinity.ImageCreator/issues">Request Feature</a>
  </p>
</div>

<div align="center">
  <a href="https://discord.gg/GVUXMNv7vV">
    <img src="https://discord.com/api/guilds/875716592770637824/widget.png" alt="Discord">
  </a>
</div>

<br>
<br>



# Count To Infinity

A small CLI tool that generates image files for sequential numbers by compositing digit sprites onto a template. Designed for simplicity, speed and easy customization.

- Generates PNG images for every number in a user-specified range.
- Caches and reuses resized digit images for better performance.
- Cross-platform: runs on Windows / macOS / Linux with .NET 7+ and ImageSharp.
- Output folder is `out` by default.

Follow the project on Instagram: [Count It To Infinity – countingjourney](https://www.instagram.com/countingjourney/)

---

## Features

- Fast generation using a digit cache to avoid repeated disk I/O and resizing.
- Simple resource layout: place `clear.png` and `0.png`…`9.png` in `resources/`.
- Colorized console logs and robust input validation.
- Optional automatic opening of the output folder after generation (cross-platform).

---

## Requirements

- .NET SDK 7.0 or later
- SixLabors.ImageSharp (and related packages) — included via NuGet
- A resources folder containing:
  - `clear.png` (template/background)
  - `0.png`, `1.png`, … `9.png` (digit sprites)

---

## Quick start

1. Clone the repository or copy the project files.
2. Ensure the resources exist:

   ```
   /resources
     clear.png
     0.png
     1.png
     ...
     9.png
   ```

3. Build and run:

   ```bash
   dotnet build
   dotnet run --project CountToInfinity
   ```

4. Follow the interactive prompts:
   - Enter the starting number (From).
   - Enter the ending number (To).
   - The program generates PNG files into the `out/` folder.

---

## Usage example

- Generate images from 1 to 100:

  Run the program and enter:

  ```
  From (start number): 1
  To (end number): 100
  ```

- After generation you will find files at `out/1.png`, `out/2.png`, … `out/100.png`.

- To automatically open the output folder (Windows example):

  ```csharp
  OpenOutputFolder("out");
  ```

---

## Configuration

Key constants are defined near the top of Program.cs:

- `DigitSize` — size (px) to which digit sprites are resized (default 76).
- `ResourceFolder` — folder for `clear.png` and digit images (default `resources`).
- `OutFolder` — output folder (default `out`).

---

## Notes & tips

- Keep digit sprite filenames exactly `0.png` … `9.png` and the template as `clear.png`.
- The program clones cached digit images before drawing to avoid mutating the shared cache.
- If you expect very large batches and memory becomes a concern, you can:
  - Avoid caching the template image and load it per-file (current default).
  - Reduce `DigitSize` to lower memory usage.
- If a digit image is missing the program logs a warning and skips that digit in the composed image.

---

## Troubleshooting

- "Template image not found" — ensure `resources/clear.png` exists and path is correct.
- "Digit resource missing" — ensure `resources/0.png` … `9.png` are present.
- If `xdg-open` is not available on Linux, install `xdg-utils` or open the folder manually.

---

## Contributing

Contributions are welcome. Suggested improvements:

- Add optional zero-padding for consistent filenames and alignment.
- Add command-line flags to run non-interactively (e.g., `--from`, `--to`, `--open`).
- Add multi-threaded generation for very large ranges while respecting memory.

Please open issues or PRs on the repository.

---

## License

This project uses an open source-friendly license. Add your chosen license file (for example, MIT) to the repo.

---

## Contact

Follow and support: [Count It To Infinity – countingjourney on Instagram](https://www.instagram.com/countingjourney/)
