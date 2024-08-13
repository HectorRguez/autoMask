# autoMask
This repository provides an automatic tool to design photolithography masks in a *.cif* format for the fabrication of Microelectrode Arrays. Furthermore, it provides a complete lightweight framework for *.cif* file visualization and generation.  


## Table of contents
- [Getting started](#getting-started)
- [Software requirements](#software-requirements)
- [Project structure](#getting-started)
- [Execution flow](#execution-flow)
- [HW Architecture](#hw-architecture)

## Software requirements
* **IDE**: This project has been created in [Visual Studio Community 2022](https://visualstudio.microsoft.com/). It uses [Windows Presentation Foundation .NET Version 8.0](https://learn.microsoft.com/en-us/dotnet/desktop/wpf/overview/?view=netdesktop-8.0)

* **OS**: The project has been tested on Windows 10 and 11.

## Getting started
1. Download the latest release on the release section.
2. **Execute** the *.exe* file to run the program. 
1. The button **Read File** can be used to visualize photolithography masks.
2. The button **Generate File** opens a pop-up window that can be used to indicate the configurable mask parameters. The **+** and **-** can be used to introduce new masks or eliminate existing ones. Finally, before generating a new mask with **Generate**, the configuration can be stored in a *.csv* file for future executions by pressing **Save masks**.
3. Finally, **Configuration** opens the configuration window, that includes: 
    - Selection of the **paths** where the generated masks and a report containing the physical characteristics of every wire will be stored. 
    - **Deposition height and material**, which will be used to compute the estimated resistance.
    $$R[\Omega] = \frac{L[\text{m}]}{A[\text{m}^2]} \cdot \sigma[\text{Sm}]$$
    - 


4. Once a file has been read or generated, the right side of the screen will display relevant information.
    - The first tab contains the **contents** of the generated file in plain text.
    - The following tabs display the **wire characteristics** of every generated chip that conforms the mask design. The first columns contain information regarding the **length** of each segment. The following ones indicate the  track **widths**, and finally, an estimation of the **resistance** using the aforementioned parameters.