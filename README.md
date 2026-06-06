# Chat App

This is a simple chat application made in C# using .NET, Avalonia UI, TCP sockets, and SQL Server.

The project has two main parts: a server and a desktop client. The client connects to the server, and users can register, log in, add friends, and send messages to each other.

Features

    User registration and login
    Searching users by username
    Sending friend requests
    Accepting or rejecting friend requests
    Removing friends
    Sending and receiving chat messages
    Loading previous messages from the database
    Real-time updates for messages and friend requests

Technologies Used

    C#
    .NET 10
    Avalonia UI
    SQL Server
    TCP sockets
    JSON serialization

Project Structure

    Klijent/ - desktop client application
    Server/ - server application
    Zajednicki/ - shared classes used by both client and server
    BrokerBazePodataka/ - database broker logic
    SystemOperationsBase/ - system operations, such as login, registration, sending messages, etc.
    baza.sql - SQL script for creating the database
    docker-compose.yaml - SQL Server Docker setup

How It Works

The client communicates with the server using TCP sockets.

One socket is used for normal requests, such as login, registration, searching users, and sending messages. Another socket is used for push notifications, so the server can send new messages and friend request updates to online users immediately.

Data such as users, friendships, and messages is stored in SQL Server.

Notes

This project is mainly made for learning purposes. Passwords are currently stored as plain text, so the application should not be available to the public.
