﻿namespace ProductService.EventProcessing;

public interface IEventProcessor
{
    Task ProcessEvent(string message);
}
