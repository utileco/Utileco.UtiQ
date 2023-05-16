﻿namespace Utileco.UtiQ
{
    public delegate Task<TResponse> RequestHandlerDelegate<TResponse>();

    public delegate Task RequestHandlerDelegate();

    public interface IPipelineBehavior<in TRequest, TResponse> where TRequest : notnull
    {
        Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken);
    }

    public interface IPipelineBehavior<in TRequest> where TRequest : notnull
    {
        Task Handle(TRequest request, RequestHandlerDelegate next, CancellationToken cancellationToken);
    }
}
