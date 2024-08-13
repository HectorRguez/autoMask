# autoMask
This repository provides an automatic tool to design photolithography masks in a *.cif* format for the fabrication of Microelectrode Arrays (MEAs). Furthermore, it provides a complete lightweight framework for *.cif* file visualization and generation.

<div align="center">
    <img src="https://github.com/hector/autoMask/main/docs/mask_example.png" width="400">
</div>

## Table of contents
- [Getting started](#getting-started)
- [How to contribute](#how-to-contribute)
- [Execution flow](#execution-flow)
- [HW Architecture](#hw-architecture)

## Getting started
1. Download the latest release on the release section.
2. **Execute** the *.exe* file to run the program. 
1. The button **Read File** can be used to visualize photolithography masks.
2. The button **Generate File** opens a pop-up window that can be used to indicate the configurable mask parameters. The **+** and **-** can be used to introduce new masks or eliminate existing ones. Finally, before generating a new mask with **Generate**, the configuration can be stored in a *.csv* file for future executions by pressing **Save masks**.
3. Finally, **Configuration** opens the configuration window, that includes: 
    - Selection of the **paths** where the generated masks and a report containing the physical characteristics of every wire will be stored. 
    - **Deposition height and material**, which will be used to compute the estimated resistance.
    $$R[\Omega] = \frac{L[\text{m}]}{A[\text{m}^2]} \cdot \sigma[\text{Sm}]$$
    - The wire width of the segment that is closer to the electrodes has a fixed width. However, the next two widths can be adjusted to minimize the resistance of the electrodes that are placed the furthest away from the interconnection pads. The function **Optimize wire width** generates multiple masks to determine the optimal solution.
    - If **Equilibrate resistances** is enabled, the width furthest segment will be adjusted to ensure that the track of every electrode has the same resistance. This implies that the pads that are placed closer to the MEA will have thinner wiring.
4. Once a file has been read or generated, the right side of the screen will display relevant information.
    - The first tab contains the **contents** of the generated file in plain text.
    - The following tabs display the **wire characteristics** of every generated chip that conforms the mask design. The first columns contain information regarding the **length** of each segment. The following ones indicate the  track **widths**, and finally, an estimation of the **resistance** using the aforementioned parameters.

## How to contribute
Follow the following steps to contribute or make modifications to the program source code. 
1. Download the recommended **IDE**. This project has been created in [Visual Studio Community 2022](https://visualstudio.microsoft.com/). It uses [Windows Presentation Foundation .NET Version 8.0](https://learn.microsoft.com/en-us/dotnet/desktop/wpf/overview/?view=netdesktop-8.0)
2. Clone the repository ```git clone https://github.com/hector/autoMask/```
3. Open the project's solution, which is an *.sln* file.
4. Compile and run the project.