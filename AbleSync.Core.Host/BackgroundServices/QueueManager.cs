using AbleSync.Core.Entities;
using AbleSync.Core.Host.Exceptions;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;

namespace AbleSync.Core.Host.BackgroundServices
{
    /// <summary>
    ///    Wrapper around a queue for project tasks.
    /// </summary>
    public class QueueManager
    {
        private const uint MaxQueueSize = 2;

        private readonly Queue<ProjectTask> Queue = new Queue<ProjectTask>((int)MaxQueueSize);
        private readonly ILogger<QueueManager> _logger;

        /// <summary>
        ///     Create new instance.
        /// </summary>
        public QueueManager(ILogger<QueueManager> logger)
            => _logger = logger ?? throw new ArgumentNullException(nameof(logger));

        /// <summary>
        ///     Enqueue an item to be executed by our framework.
        /// </summary>
        /// <param name="projectTask">The task to be executed.</param>
        public void Enqueue(ProjectTask projectTask)
        {
            if (Queue.Count > MaxQueueSize)
            {
                throw new QueueFullException();
            }

            Queue.Enqueue(projectTask);
            _logger.LogTrace($"Enqueued task {projectTask.Id}");
        }

        /// <summary>
        ///     Gets the item count in our queue.
        /// </summary>
        /// <returns>The amount of items in the queue.</returns>
        public uint GetCount()
            => (uint)Queue.Count;

        // TODO What if we don't have any items?
        /// <summary>
        ///     Dequeues an item from our queue.
        /// </summary>
        /// <returns>The dequeued item.</returns>
        public ProjectTask Dequeue()
            => Queue.Dequeue();
    }
}
