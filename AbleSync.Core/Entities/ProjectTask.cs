using AbleSync.Core.Types;
using System;

namespace AbleSync.Core.Entities
{
    /// <summary>
    ///     Class that represents a processing task for a <see cref="Project"/>.
    /// </summary>
    public class ProjectTask : EntityBase, IEntityAudit
    {
        /// <summary>
        ///     The referenced <see cref="Project"/> to which this task belongs.
        /// </summary>
        public Guid ProjectId { get; set; }

        /// <summary>
        ///     Creation time.
        /// </summary>
        public DateTimeOffset? DateCreated { get; set; }

        /// <summary>
        ///     Latest update time.
        /// </summary>
        public DateTimeOffset? DateUpdated { get; set; }

        /// <summary>
        ///     Task completion time.
        /// </summary>
        public DateTimeOffset? DateCompleted { get; set; }

        /// <summary>
        ///     Indicates the status of this project task.
        /// </summary>
        public ProjectTaskStatus ProjectTaskStatus { get; set; }

        /// <summary>
        ///     Indicates the type of task.
        /// </summary>
        public ProjectTaskType ProjectTaskType { get; set; }
        
        // TODO This is not bullet proof.
        /// <summary>
        ///     Parameter for this project task.
        /// </summary>
        public string TaskParameter { get; set; }

        /// <summary>
        ///     Represents whether or not this item has been completed.
        /// </summary>
        public bool Completed => ProjectTaskStatus == ProjectTaskStatus.Done || ProjectTaskStatus == ProjectTaskStatus.Failed;
    }
}
