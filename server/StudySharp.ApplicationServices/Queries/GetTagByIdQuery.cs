﻿using System.Threading;
using System.Threading.Tasks;
using MediatR;
using StudySharp.Domain.Constants;
using StudySharp.Domain.General;
using StudySharp.Domain.Models;
using StudySharp.DomainServices;

namespace StudySharp.ApplicationServices.Queries
{
    public sealed class GetTagByIdQuery : IRequest<OperationResult<Tag>>
    {
        public int Id { get; set; }
    }

    public sealed class GetTagByIdQueryHandler : IRequestHandler<GetTagByIdQuery, OperationResult<Tag>>
    {
        private readonly StudySharpDbContext _studySharpDbContext;

        public GetTagByIdQueryHandler(StudySharpDbContext studySharpDbContext)
        {
            _studySharpDbContext = studySharpDbContext;
        }

        public async Task<OperationResult<Tag>> Handle(GetTagByIdQuery request, CancellationToken cancellationToken)
        {
            var tag = await _studySharpDbContext.Tags.FindAsync(request.Id, cancellationToken);
            if (tag == null)
            {
                return OperationResult.Fail<Tag>(string.Format(ErrorConstants.EntityNotFound, nameof(Tag), nameof(Tag.Id), request.Id));
            }

            return OperationResult.Ok(tag);
        }
    }
}
