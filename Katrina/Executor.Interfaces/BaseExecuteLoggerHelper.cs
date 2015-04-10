﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using AV.Cyclone.Katrina.Executor.Interfaces;

namespace AV.Cyclone.Katrina.Executor
{
    public class BaseExecuteLoggerHelper : IExecuteLoggerHelper
    {
        private readonly IExecuteLogger executeLogger;

        public BaseExecuteLoggerHelper(IExecuteLogger executeLogger)
        {
            this.executeLogger = executeLogger;
        }

        public virtual T LogAssign<T>(string expression, string fileNme, int lineNumber, T value)
        {
            executeLogger.LogAssign(expression, fileNme, lineNumber, value);
            return value;
        }

        public virtual T LogPostIncrement<T>(string expression, string fileNme, int lineNumber, T result, T value)
        {
            executeLogger.LogAssign(expression, fileNme, lineNumber, value);
            return result;
        }

        public virtual void BeginLoop(string fileName, int lineNumber)
        {
            executeLogger.BeginLoop(fileName, lineNumber);
        }

        public virtual void LoopIteration(string fileName, int lineNumber)
        {
            executeLogger.LoopIteration(fileName, lineNumber);
        }

        public virtual T LoopIteration<T>(string expression, string fileName, int lineNumber, T value)
        {
            executeLogger.LogAssign(expression, fileName, lineNumber, value);
            executeLogger.LoopIteration(fileName, lineNumber);
            return value;
        }

        public virtual void EndLoop(string fileName, int lineNumber)
        {
            executeLogger.EndLoop(fileName, lineNumber);
        }

        public virtual void BeginMethod(string methodName, string fileName, int lineNumber)
        {
            executeLogger.BeginMethod(methodName, fileName, lineNumber);
        }

        public virtual void EndMethod(string methodName, string fileName, int lineNumber)
        {
            executeLogger.EndMethod(methodName, fileName, lineNumber);
        }

        public virtual void CatchException(Exception exception)
        {
            var stackTrace = new StackTrace(exception, true);
            var frame = stackTrace.GetFrame(stackTrace.FrameCount - 1);
            var fileName = frame.GetFileName();
            var lineNumber = frame.GetFileLineNumber();
            executeLogger.LogAssign("exception", fileName, lineNumber - 1, exception.Message);
        }
    }
}