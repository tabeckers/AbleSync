using AbleSync.Core.Entities;
using AbleSync.Core.Host.Exceptions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;

namespace AbleSync.Core.Host.BackgroundServices
{
    /// <summary>
    ///    Wrapper around a queue for project tasks.
    /// </summary>
    public class QueueManager
    {
        private readonly uint MaxQueueSize;

        private readonly Queue<ProjectTask> Queue;
        private readonly ILogger<QueueManager> _logger;

        /// <summary>
        ///     Create new instance.
        /// </summary>
        public QueueManager(IOptions<AbleSyncOptions> options,
            ILogger<QueueManager> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));

            MaxQueueSize = options?.Value.TaskExecutionQueueSize ?? throw new ArgumentNullException(nameof(options));
            if (MaxQueueSize == 0 || MaxQueueSize >= int.MaxValue)
            {
                throw new ArgumentOutOfRangeException(nameof(options.Value.TaskExecutionQueueSize));
            }

            // Initialize the queue with correct size dynamically.
            Queue = new Queue<ProjectTask>((int)MaxQueueSize);
        }

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
