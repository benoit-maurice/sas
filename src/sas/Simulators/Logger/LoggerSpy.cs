﻿using Microsoft.Extensions.Logging;

namespace sas.Simulators.Logger;

public interface ISpyLogs
{
    bool WasNeverCalled(LogLevel? specificLevelToCheck = null);

    bool WasCalledWith(params (LogLevel level, string message)[] expected);

    bool WasCalledOnlyWith(params (LogLevel level, string message)[] expected);
}

internal class LoggerSpy<T> : ILogger<T>, ISpyLogs
{
    private record Scope<TState>(TState _) : IDisposable
    {
        public void Dispose() { }
    }

    #if NET7_0_OR_GREATER
    public IDisposable? BeginScope<TState>(TState state) where TState : notnull
    #else 
    public IDisposable BeginScope<TState>(TState state)
    #endif
    {
        return new Scope<TState>(state);
    }

    public bool IsEnabled(LogLevel logLevel)
    {
        return true;
    }

    private readonly List<(LogLevel Level, string Message)> _logs = new();

    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
    {
        _logs.Add((logLevel, $"{state}"));
    }

    public bool WasNeverCalled(LogLevel? specificLevelToCheck = null)
    {
        if (specificLevelToCheck is not null)
        {
            var logsToCheck = _logs.Where(log => log.Level == specificLevelToCheck);
            return !logsToCheck.Any();
        }

        return !_logs.Any();
    }

    public bool WasCalledWith(params (LogLevel level, string message)[] expected)
    {
        return expected.All(expectedLog => _logs.Contains(expectedLog));
    }

    public bool WasCalledOnlyWith(params (LogLevel level, string message)[] expected)
    {
        return expected.All(expectedLog => _logs.Contains(expectedLog)) 
               && _logs.All(log => expected.Contains(log));
    }
}

internal class LoggerSpy : ILogger, ISpyLogs
{
    private record Scope<TState>(TState _) : IDisposable
    {
        public void Dispose() { }
    }

    #if NET7_0_OR_GREATER
    public IDisposable? BeginScope<TState>(TState state) where TState : notnull
    #else 
    public IDisposable BeginScope<TState>(TState state)
    #endif
    {
        return new Scope<TState>(state);
    }

    public bool IsEnabled(LogLevel logLevel)
    {
        return true;
    }

    private readonly List<(LogLevel Level, string Message)> _logs = new();

    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
    {
        _logs.Add((logLevel, $"{state}"));
    }

    public bool WasNeverCalled(LogLevel? specificLevelToCheck = null)
    {
        if (specificLevelToCheck is not null)
        {
            var logsToCheck = _logs.Where(log => log.Level == specificLevelToCheck);
            return !logsToCheck.Any();
        }

        return !_logs.Any();
    }

    public bool WasCalledWith(params (LogLevel level, string message)[] expected)
    {
        return expected.All(expectedLog => _logs.Contains(expectedLog));
    }

    public bool WasCalledOnlyWith(params (LogLevel level, string message)[] expected)
    {
        return expected.All(expectedLog => _logs.Contains(expectedLog)) 
               && _logs.All(expected.Contains);
    }
}