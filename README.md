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


## One-line summary

```text
[Communicating Software] ? VP1 ? VP2 ? [SerialSniffer] ? P1 ? [Connected Device]; com0com provides VP1?VP2 (virtual null-modem); SerialSniffer bridges VP2?P1; full-duplex relay, timestamps + logs (HEX/ASCII/Binary), optional export (.txt/.doc).
```

## Detailed workflow

```text

WORKFLOW — Serial Monitoring with com0com (Virtual) + Physical Device (Detailed)

COMPONENTS
- Communicating Software: any Windows app that opens a COM port (test tool, client, HIL app).
- com0com: installs a signed virtual null-modem pair VP1 <-> VP2, internally connected.
- SerialSniffer: Port Monitor's bridge/logger that opens VP2 and P1 concurrently.
- P1: real hardware COM/USB-Serial port connected to the Physical Device.
- Connected Device: external target (sensor, PLC, MCU, modem, etc.).

SESSION STARTUP
1) com0com pair is present (VP1, VP2). If missing, installer can deploy it.
2) Communicating Software opens VP1 with desired settings (baud, parity, data bits, stop bits, flow control).
3) SerialSniffer opens VP2 and P1 using matching serial parameters to ensure transparent forwarding.
4) Logging session begins (timestamp source: system monotonic clock; direction markers enabled).

FORWARD PATH (Software ? Device)
A) Software writes bytes to VP1.
B) com0com forwards bytes VP1 ? VP2 internally (zero-copy from user perspective).
C) SerialSniffer reads from VP2, annotates timestamp + direction [S?D], optional decode view (HEX/ASCII/Binary).
D) SerialSniffer writes the same buffer to P1.
E) Device receives data via the physical link.

RETURN PATH (Device ? Software)
F) Device transmits bytes on P1.
G) SerialSniffer reads from P1, annotates timestamp + direction [D?S], optional decode view (HEX/ASCII/Binary).
H) SerialSniffer writes the same buffer to VP2.
I) com0com forwards bytes VP2 ? VP1.
J) Software reads bytes from VP1.

INVARIANTS
- Payload-transparent: SerialSniffer does not modify bytes; only relays + logs.
- Full-duplex: simultaneous S?D and D?S streams are handled.
- Back-pressure: if destination write blocks, the relay waits or buffers according to app settings.
- Error propagation: port errors (break, framing, parity) are logged and surfaced to UI when available.
- Parameter sync: VP2 and P1 are opened with identical serial settings for symmetric behavior.

LOGGING & EXPORT
- Each frame recorded with: timestamp, direction, port name, byte count, and selected view.
- Log rotation configurable (size/time based).
- Export supported: .txt (raw/decoded) and .doc (formatted session report).

COMMON MODES
- Physical capture: use P1 + target device; Sniffer taps at P1 while Software may still use VP1/VP2 for test loops.
- Software-only lab/CI: no hardware; Software on VP1, Sniffer on VP2; optional synthetic responders on P1 omitted.

QUICK DIAGNOSTIC STRING
Software?Device: Software?VP1?VP2?Sniffer?P1?Device
Device?Software: Device?P1?Sniffer?VP2?VP1?Software

```

## Mermaid sequence


```mermaid
sequenceDiagram
    autonumber
    participant SW as Communicating Software
    participant VP1 as VP1 (com0com)
    participant VP2 as VP2 (com0com)
    participant SN as SerialSniffer
    participant P1 as P1 (Real COM)
    participant DEV as Connected Device

    note over VP1,VP2: com0com creates virtual null-modem pair (VP1?VP2)

    SW->>VP1: Open & Configure (baud/parity/bits)
    SW->>VP1: TX bytes
    VP1-->>VP2: Forward (virtual link)
    VP2->>SN: RX (capture + timestamp [S?D])
    SN->>P1: Forward to hardware
    P1->>DEV: TX to device

    DEV-->>P1: RX from device
    P1->>SN: RX (capture + timestamp [D?S])
    SN->>VP2: Forward to virtual
    VP2-->>VP1: Forward (virtual link)
    VP1-->>SW: RX to software

    note over SN: Log both directions; views: HEX/ASCII/Binary; export: .txt/.doc
```

## Mermaid flowchart


```mermaid
flowchart LR
  SW[Communicating Software] <---> VP1[VP1 (com0com)]
  VP1 <-- Virtual Pair --> VP2[VP2 (com0com)]
  VP2 <--> SN[SerialSniffer (Bridge + Logger)]
  SN <--> P1[P1 (Real COM Port)]
  P1 <--> DEV[Connected Device]

  classDef phys fill:#ffd6d6,stroke:#e88,stroke-width:1px
  classDef virt fill:#d6faff,stroke:#6ad,stroke-width:1px
  classDef snif fill:#f6f6f6,stroke:#999,stroke-width:1px
  class VP1,VP2 virt
  class P1,DEV phys
  class SN snif
```

## JSON spec


```json
{
  "components": {
    "software": "Communicating Software",
    "sniffer": "SerialSniffer",
    "virtualPorts": ["VP1", "VP2"],
    "physicalPort": "P1",
    "device": "Connected Device",
    "driver": "com0com (virtual null-modem pair)"
  },
  "serialParameters": {
    "baudRate": "match across VP2 and P1 (e.g., 9600/115200)",
    "parity": "None/Even/Odd (must match)",
    "dataBits": 8,
    "stopBits": 1,
    "flowControl": "None/RTSCTS/XONXOFF"
  },
  "flows": [
    {
      "name": "SoftwareToDevice",
      "path": ["Software", "VP1", "VP2", "SerialSniffer", "P1", "Device"],
      "log": ["timestamp", "direction:S?D", "port", "byteCount", "hex", "ascii", "binary"]
    },
    {
      "name": "DeviceToSoftware",
      "path": ["Device", "P1", "SerialSniffer", "VP2", "VP1", "Software"],
      "log": ["timestamp", "direction:D?S", "port", "byteCount", "hex", "ascii", "binary"]
    }
  ],
  "behavior": {
    "relayMode": "transparent",
    "modifiesPayload": false,
    "errorHandling": ["parity", "framing", "overrun", "break"],
    "backPressure": "blocking/queue per settings",
    "export": [".txt", ".doc"]
  }
}
```

