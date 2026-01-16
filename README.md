A simple real-time chat application consisting of a server and multiple clients. Each client is a WPF-based Windows desktop application that connects to the server over TCP sockets to send and receive chat messages asynchronously. Communication is handled using low-level sockets, enabling real-time message delivery while keeping the UI responsive.

The client interface displays the message history and provides an input field with a Send button for composing messages. Incoming messages are received and rendered in real time without polling or refresh actions.

Key Features

Multi-Client Chat
Multiple clients can be connected at the same time and exchange messages in real time.

WPF User Interface
The client is built using Windows Presentation Foundation (WPF) to provide a native Windows desktop experience.

Asynchronous Communication
All networking operations are asynchronous, ensuring that sending and receiving messages never block the UI thread.

Real-Time Message Delivery
Messages are delivered instantly as they are received by the server, without manual refresh or background polling.

Low-Level TCP Communication
The application uses raw TCP sockets for reliable client–server communication.

Technology Stack

C# (.NET)
The application is implemented in C# using the .NET platform.

WPF
Windows Presentation Foundation is used for the desktop client UI.

TCP Sockets
Communication between clients and the server is handled using low-level TCP sockets.