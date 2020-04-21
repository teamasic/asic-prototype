using AttendanceSystemIPCamera.Framework.ViewModels;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AttendanceSystemIPCamera.Framework.Validators
{
    public class GroupValidator
    {
        public class CreateGroupValidator: AbstractValidator<GroupViewModel>
        {
            public CreateGroupValidator()
            {
                RuleFor(group => group.Code).NotNull().NotEmpty().Length(1, 50);
                RuleFor(group => group.Name).NotNull().NotEmpty().Length(3, 100);
                RuleFor(group => group.TotalSession).NotNull().GreaterThan(0).LessThan(100);
                RuleFor(group => group.Attendees).NotNull().NotEmpty();
            }
        }

        public class UpdateGroupValidator : AbstractValidator<GroupViewModel>
        {
            public UpdateGroupValidator()
            {
                RuleFor(group => group.Code).NotNull().NotEmpty().Length(1, 50);
                RuleFor(group => group.Name).NotNull().NotEmpty().Length(3, 100);
                RuleFor(group => group.TotalSession).NotNull().GreaterThan(0).LessThan(100);
            }
        }
    }
}
