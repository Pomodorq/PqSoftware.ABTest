using FluentValidation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace PqSoftware.ABTest.Data.Dto
{
    public class PostProjectUserRequest
    {
        [Required]
        public int? UserId { get; set; }

        [Required]
        public int? ProjectId { get; set; }
        [Required]
        public DateTime? DateRegistration { get; set; }
        [Required]
        public DateTime? DateLastActivity { get; set; }
    }

    public class PostProjectUserRequestValidator: AbstractValidator<PostProjectUserRequest>
    {
        public PostProjectUserRequestValidator()
        {
            RuleFor(projectUser => projectUser.UserId)
                .NotNull();
            RuleFor(projectUser => projectUser.ProjectId)
                .NotNull();
            RuleFor(projectUser => projectUser.DateRegistration)
                .NotNull()
                .GreaterThan(new DateTime(1970, 1, 1));
            RuleFor(projectUser => projectUser.DateLastActivity)
                .NotNull()
                .GreaterThan(new DateTime(1970, 1, 1));
        }
    }
}
