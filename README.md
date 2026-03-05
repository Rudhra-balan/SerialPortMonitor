# Serial Port Monitor

## Free & Open Source COM Port Monitoring Tool

A lightweight Serial Port Monitoring and Communication Tool built as a free alternative to expensive commercial Serial Port Monitor software.

This application allows developers, embedded engineers, and testers to monitor, debug, analyze, and simulate serial communication between Windows applications and hardware devices.

It also integrates with C3 / com0com virtual ports to simulate serial communication between applications without requiring physical hardware.

## Why This Project Exists

Many serial port monitoring tools require **paid licenses or subscriptions**. This project provides a **free, open-source solution with essential features** needed for:

- Embedded system debugging
- Hardware communication testing
- Protocol analysis
- Serial data logging
- Virtual COM port testing

## Features
**Serial Port Monitoring (Sniffer Mode)**
- Monitor data exchanged between:
- Windows applications
- External hardware devices
- Virtual COM ports
- Displays real-time incoming and outgoing serial data.

## Multiple Data Formats

View serial data in multiple formats:
- HEX
- ASCII
- Binary

This helps analyze different device protocols and packet structures.

## Data Logging
Record serial communication for debugging and analysis.
- Logs all incoming and outgoing bytes
- Useful for high-speed communication capture
- Export logs for offline analysis

## Data Transmission

Send data manually to the serial port.

Useful for:
- Device testing
- Command execution
- Protocol validation

## Terminal Mode

A built-in **serial terminal interface** that allows you to:

- Send commands to connected devices
- Receive device responses
- Test serial communication without external tools

## Virtual COM Port Support

Supports virtual serial ports using:

C3 / com0com

This allows developers to simulate serial communication between two applications.

**Example:**

**Application A ? COM5
Application B ? COM6**

Both ports are linked using com0com, allowing data simulation without hardware.

# How It Works

The application connects to a *serial port or virtual port pair*, captures communication data, processes it, and displays it in real time.

# Software

Windows OS

- .NET Runtime

- Visual Studio (for development)

# Virtual Port Driver

*Install:*

com0com from Build/Vitrual/PortManager.exe

# workflow
```csharp
+-----------------------+
|  External Device      |
|  (Microcontroller)    |
+-----------+-----------+
            |
            | Serial Communication
            |
+-----------v-----------+
|   Serial Port Driver  |
|   (COM Port)          |
+-----------+-----------+
            |
            |
+-----------v-----------+
|  Port Monitor App     |
|                       |
|  - Sniffer Engine     |
|  - Data Parser        |
|  - Logging Engine     |
|  - UI Renderer        |
+-----------+-----------+
            |
            |
+-----------v-----------+
| Data Storage / Export |
| TXT / DOC Logs        |
+-----------------------+
```
# Virtual COM Port Workflow (com0com)
```csharp
        +------------------+
        |   Application A  |
        +---------+--------+
                  |
                  | COM5
                  |
        +---------v--------+
        |     com0com      |
        | Virtual Port Pair|
        +---------+--------+
                  |
                  | COM6
                  |
        +---------v--------+
        |   Application B  |
        +------------------+
   ```     
        
## Application Privileges

**Administrator Mode**

Full features enabled:
- Serial monitoring
- Port communication
- Data capture

**Non-Administrator Mode**

Limited features:
- Serial communication only
  
![Screenshot](1.png)
![Screenshot](2.png)

