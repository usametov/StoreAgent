﻿using System.Runtime.CompilerServices;
using StoreAgent;

public sealed class Program {
    public static void Main(string[] args) {
        var dispatcher = new Dispatcher();
        dispatcher.Init();
        dispatcher.LoadProducts();
        dispatcher.StartVendingMachine();
    }
}