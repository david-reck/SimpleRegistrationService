// <auto-generated>
//     Generated by the protocol buffer compiler.  DO NOT EDIT!
//     source: Proto/RegistrationService.proto
// </auto-generated>
#pragma warning disable 0414, 1591
#region Designer generated code

using grpc = global::Grpc.Core;

namespace RegistrationService.API.Grpc {
  public static partial class RegistrationApiRetrieval
  {
    static readonly string __ServiceName = "RegistrationApi.RegistrationApiRetrieval";

    static readonly grpc::Marshaller<global::RegistrationService.API.Grpc.AdtMessageRequest> __Marshaller_RegistrationApi_AdtMessageRequest = grpc::Marshallers.Create((arg) => global::Google.Protobuf.MessageExtensions.ToByteArray(arg), global::RegistrationService.API.Grpc.AdtMessageRequest.Parser.ParseFrom);
    static readonly grpc::Marshaller<global::RegistrationService.API.Grpc.AdtMessageResponse> __Marshaller_RegistrationApi_AdtMessageResponse = grpc::Marshallers.Create((arg) => global::Google.Protobuf.MessageExtensions.ToByteArray(arg), global::RegistrationService.API.Grpc.AdtMessageResponse.Parser.ParseFrom);

    static readonly grpc::Method<global::RegistrationService.API.Grpc.AdtMessageRequest, global::RegistrationService.API.Grpc.AdtMessageResponse> __Method_FindAdtMessageById = new grpc::Method<global::RegistrationService.API.Grpc.AdtMessageRequest, global::RegistrationService.API.Grpc.AdtMessageResponse>(
        grpc::MethodType.Unary,
        __ServiceName,
        "FindAdtMessageById",
        __Marshaller_RegistrationApi_AdtMessageRequest,
        __Marshaller_RegistrationApi_AdtMessageResponse);

    /// <summary>Service descriptor</summary>
    public static global::Google.Protobuf.Reflection.ServiceDescriptor Descriptor
    {
      get { return global::RegistrationService.API.Grpc.RegistrationServiceReflection.Descriptor.Services[0]; }
    }

    /// <summary>Base class for server-side implementations of RegistrationApiRetrieval</summary>
    [grpc::BindServiceMethod(typeof(RegistrationApiRetrieval), "BindService")]
    public abstract partial class RegistrationApiRetrievalBase
    {
      public virtual global::System.Threading.Tasks.Task<global::RegistrationService.API.Grpc.AdtMessageResponse> FindAdtMessageById(global::RegistrationService.API.Grpc.AdtMessageRequest request, grpc::ServerCallContext context)
      {
        throw new grpc::RpcException(new grpc::Status(grpc::StatusCode.Unimplemented, ""));
      }

    }

    /// <summary>Creates service definition that can be registered with a server</summary>
    /// <param name="serviceImpl">An object implementing the server-side handling logic.</param>
    public static grpc::ServerServiceDefinition BindService(RegistrationApiRetrievalBase serviceImpl)
    {
      return grpc::ServerServiceDefinition.CreateBuilder()
          .AddMethod(__Method_FindAdtMessageById, serviceImpl.FindAdtMessageById).Build();
    }

    /// <summary>Register service method with a service binder with or without implementation. Useful when customizing the  service binding logic.
    /// Note: this method is part of an experimental API that can change or be removed without any prior notice.</summary>
    /// <param name="serviceBinder">Service methods will be bound by calling <c>AddMethod</c> on this object.</param>
    /// <param name="serviceImpl">An object implementing the server-side handling logic.</param>
    public static void BindService(grpc::ServiceBinderBase serviceBinder, RegistrationApiRetrievalBase serviceImpl)
    {
      serviceBinder.AddMethod(__Method_FindAdtMessageById, serviceImpl == null ? null : new grpc::UnaryServerMethod<global::RegistrationService.API.Grpc.AdtMessageRequest, global::RegistrationService.API.Grpc.AdtMessageResponse>(serviceImpl.FindAdtMessageById));
    }

  }
}
#endregion