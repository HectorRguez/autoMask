# autoMask
This repository provides an automatic tool to design photolithography masks in a *.cif* format for the fabrication of Microelectrode Arrays (MEAs). Furthermore, it provides a complete framework for *.cif* file visualization and generation.

Optimizing MEA design involves a thorough design space exploration. The large number of parameters requires multiple iterations of the designs and fabrication processes. The goal of this project is to facilitate this process by **automating the design step**. MEAs are repetitive and, usually, symmetrical. Thus, providing designs after selecting a few distances results in excellent time savings.

Multiple designs, which will also be called *chips*, can fit in a single silicon wafer. However, this can dramatically increase the number of polygons that compose each mask. The already available mask design solutions, such as [CleWin 4.0]() are unable to handle this increased complexity, especially when visualizing the complete mask. This software cashes the corresponding vectorial image to visualize the masks without requiring large amounts of resources. The goal is to be able to provide a complete picture of the to be manufactured mask, which can be zoomed in or out and panned in a responsive manner.  

<div align="center">
    <img src="https://github.com/HectorRguez/autoMask/blob/master/docs/mask_example.png" width="400">
</div>

## Table of contents
- [Getting started](#getting-started)
- [How to contribute](#how-to-contribute)
- [Project structure](#project-structure)
- [Configuration parameters](#configuration-parameters)
- [cifFile class](#cif-file-class)


## Getting started
1. Download the latest release on the release section.
2. **Execute** the *.exe* file to run the program. 
1. The button **Read File** can be used to visualize photolithography masks. A *.cif* mask example is contained in the *docs* folder.
2. The button **Generate File** opens a pop-up window that can be used to indicate the configurable mask parameters. The **+** and **-** can be used to introduce new masks or eliminate existing ones. Finally, before generating a new mask with **Generate**, the configuration can be stored in a *.csv* file for future executions by pressing **Save masks**. An example mask configuration is included in the release, inside the file *config_mask.csv*. 
3. Finally, **Configuration** opens the configuration window, that includes: 
    - Selection of the **paths** where the generated masks and a report containing the physical characteristics of every wire will be stored. 
    - **Deposition height and material**, which will be used to compute the estimated resistance.
    $$R[\Omega] = \frac{L[\text{m}]}{A[\text{m}^2]} \cdot \sigma[\frac{\text{S}}{m}]$$
    - The wire width of the segment that is closer to the electrodes has a fixed width. However, the next two widths can be adjusted to minimize the resistance of the electrodes that are placed the furthest away from the interconnection pads. The function **Optimize wire width** generates multiple masks to determine the optimal solution.
    - If **Equilibrate resistances** is enabled, the width furthest segment will be adjusted to ensure that the track of every electrode has the same resistance. This implies that the pads that are placed closer to the MEA will have thinner wiring.
4. Once a file has been read or generated, the right side of the screen will display relevant information.
    - The first tab contains the **contents** of the generated file in plain text.
    - The following tabs display the **wire characteristics** of every generated chip that conforms the mask design. It contains information regarding the **length** and **width** of each segment, as well as the total length and an estimation of the **resistance** using the aforementioned expression.

## How to contribute
Follow the following steps to contribute or make modifications to the program source code. 
1. Download the **IDE**. This project has been created in [Visual Studio Community 2022](https://visualstudio.microsoft.com/). It uses [Windows Presentation Foundation .NET Version 8.0](https://learn.microsoft.com/en-us/dotnet/desktop/wpf/overview/?view=netdesktop-8.0)
2. Clone the repository: ```git clone https://github.com/hector/autoMask/```
3. Open the project's solution with the recommended IDE. The solution file has the  *.sln* extension.
4. Compile and run the project. [^1]

[^1] To load the example mask configuration, the *mask_config.csv* file should be copied inside the compiled project directory, which should be inside the *bin/Debug* folder.

## Project structure
<pre>
├── src
│   ├── App.xaml                # [DEFAULT] Common for every WPF app
│   ├── App.xaml.cs             # [DEFAULT] Common for every WPF app
│   ├── AssemblyInfo.cs         # [DEFAULT] Common for every WPF app
│   ├── autoMask.csproj         # Project
│   ├── AuxFun.cs               # Class that contains auxiliary functions
│   ├── Chip.cs                 # Class used to configure the generation parameters.
│   ├── cifAuto.cs              # Class used to automatically design masks
│   ├── CifFile.cs              # Class used to interpret, store and visualize masks
│   ├── ConfigWindow.xaml       # Configuration window visuals
│   ├── ConfigWindow.xaml.cs    # Configuration window functionality
│   ├── MainWindow.xaml         # Main window visuals
│   ├── MainWindow.xaml.cs      # Main window functionality
│   ├── MaskWindow.xaml         # Generate Mask window visuals
│   ├── MaskWindow.xaml.cs      # Generate Mask window functionality
│   └── ZoomBorder.cs           # Class used to implement the pan and zoom function
├── autoMask.sln                # Project solution
├── docs                        # Documentation figures
├── LICENSE
└── README.md
</pre>

## Configuration parameters
The parameters shown on this image fully represent the characteristics of each generated chip:

<div align="center">
    <img src="https://github.com/HectorRguez/autoMask/blob/master/docs/mask_parameters.png" width="600">
</div>


The values that define the general structure of the system are set up as constants in the *Chip.cs* file. The configurable parameters can be set up differently for each chip in the **Generate Mask window** [^2].

[^2] the Pad Length parameter can be calculated automatically to maximize the covered area if it is set as 0. However, if some other value is introduced it will overwrite this calculation, up to the a maximum pad length of 2 mm.
``` 
Parameter                 [um]
------------------------------
Maximum pad length        2000
Pad base                  2700
Pad height                7500
Square base               2900
Square height A           3974
Square height B           4266
Wire minimum width           5
Wire corner length         100
Square width               100
Square length              500
Reference separation      1700
Electrode number        CONFIG
Electrode distance      CONFIG
Electrode diameter      CONFIG
Wire minimum width      CONFIG
Pad length              CONFIG
------------------------------
```

An example configuration file with four chips, designed to fit in a 6 inch wafer is provided in *config_mask.csv*. The first image of this file contains a chip that has the same parameters as Chip #4.

```
Parameter              Chip #1         Chip #2        Chip #3         Chip #4
-----------------------------------------------------------------------------
Electrode number           10              20              40              80  
Electrode distance         10              10              10              10  
Electrode diameter          5               5               5               5   
Pad length               2000            2000            2000            2000
Wire minimum width          5               5               5               5   
-----------------------------------------------------------------------------
```

## cifFile class
The most important class of the project is the cifFile. It handles all the *.cif* file readings, modifications, new file generation and storage. This standard includes:
- Although there is no standard **header**, software such as CleWin usually incorporates information regarding the author of the mask. 
- **Layer declaration**: Each layer is defined with a number. Furthermore, the comment placed in the same line is used by CleWin to select the color of the specific layer.
- The content of the mask is composed of three major shapes: **Circles**, **Boxes** and **Wires**.
- This file format uses modules which will be referred to as **elements** to avoid repetition. Elements are declared as belonging to a specific layer, with a name, an index and their contents. Then, this element can be instantiated on any desired position as many times as needed throughout the file.

<div align="center">
    <img src="https://github.com/HectorRguez/autoMask/blob/master/docs/cif_file_structure.png" width="400">
</div>

