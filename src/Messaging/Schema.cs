// <auto-generated>
//     Generated by the protocol buffer compiler.  DO NOT EDIT!
//     source: schema.proto
// </auto-generated>
#pragma warning disable 1591, 0612, 3021, 8600, 8981
#region Designer generated code

using pb = global::Google.Protobuf;
using pbc = global::Google.Protobuf.Collections;
using pbr = global::Google.Protobuf.Reflection;
using scg = global::System.Collections.Generic;
namespace Schema
{

  /// <summary>Holder for reflection information generated from schema.proto</summary>
  public static partial class SchemaReflection
  {

    #region Descriptor
    /// <summary>File descriptor for schema.proto</summary>
    public static pbr::FileDescriptor Descriptor
    {
      get { return descriptor; }
    }
    private static pbr::FileDescriptor descriptor;

    static SchemaReflection()
    {
      byte[] descriptorData = global::System.Convert.FromBase64String(
          string.Concat(
            "CgxzY2hlbWEucHJvdG8SBlNjaGVtYSJ8CgtPbmVvZlVwZGF0ZRIUCgxyZWNp",
            "cGllbnRfaWQYASABKAkSDwoHZ2FtZV9pZBgCIAEoCRI8ChVhbGxfc29sZGll",
            "cl9wb3NpdGlvbnMYAyABKAsyGy5TY2hlbWEuQWxsU29sZGllclBvc2l0aW9u",
            "c0gAQggKBnVwZGF0ZSIfCgdWZWN0b3IyEgkKAXgYASABKAISCQoBeRgCIAEo",
            "AiJJChNBbGxTb2xkaWVyUG9zaXRpb25zEjIKEXNvbGRpZXJfcG9zaXRpb25z",
            "GAEgAygLMhcuU2NoZW1hLlNvbGRpZXJQb3NpdGlvbiJeCg9Tb2xkaWVyUG9z",
            "aXRpb24SCgoCaWQYASABKAQSHAoDcG9zGAIgASgLMg8uU2NoZW1hLlZlY3Rv",
            "cjISIQoIdmVsb2NpdHkYAyABKAsyDy5TY2hlbWEuVmVjdG9yMmIGcHJvdG8z"));
      descriptor = pbr::FileDescriptor.FromGeneratedCode(descriptorData,
          new pbr::FileDescriptor[] { },
          new pbr::GeneratedClrTypeInfo(null, null, new pbr::GeneratedClrTypeInfo[] {
            new pbr::GeneratedClrTypeInfo(typeof(global::Schema.OneofUpdate), global::Schema.OneofUpdate.Parser, new[]{ "RecipientId", "GameId", "AllSoldierPositions" }, new[]{ "Update" }, null, null, null),
            new pbr::GeneratedClrTypeInfo(typeof(global::Schema.Vector2), global::Schema.Vector2.Parser, new[]{ "X", "Y" }, null, null, null, null),
            new pbr::GeneratedClrTypeInfo(typeof(global::Schema.AllSoldierPositions), global::Schema.AllSoldierPositions.Parser, new[]{ "SoldierPositions" }, null, null, null, null),
            new pbr::GeneratedClrTypeInfo(typeof(global::Schema.SoldierPosition), global::Schema.SoldierPosition.Parser, new[]{ "Id", "Pos", "Velocity" }, null, null, null, null)
          }));
    }
    #endregion

  }
  #region Messages
  public sealed partial class OneofUpdate : pb::IMessage<OneofUpdate>
#if !GOOGLE_PROTOBUF_REFSTRUCT_COMPATIBILITY_MODE
      , pb::IBufferMessage
#endif
  {
    private static readonly pb::MessageParser<OneofUpdate> _parser = new pb::MessageParser<OneofUpdate>(() => new OneofUpdate());
    private pb::UnknownFieldSet _unknownFields;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public static pb::MessageParser<OneofUpdate> Parser { get { return _parser; } }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public static pbr::MessageDescriptor Descriptor
    {
      get { return global::Schema.SchemaReflection.Descriptor.MessageTypes[0]; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    pbr::MessageDescriptor pb::IMessage.Descriptor
    {
      get { return Descriptor; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public OneofUpdate()
    {
      OnConstruction();
    }

    partial void OnConstruction();

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public OneofUpdate(OneofUpdate other) : this()
    {
      recipientId_ = other.recipientId_;
      gameId_ = other.gameId_;
      switch (other.UpdateCase)
      {
        case UpdateOneofCase.AllSoldierPositions:
          AllSoldierPositions = other.AllSoldierPositions.Clone();
          break;
      }

      _unknownFields = pb::UnknownFieldSet.Clone(other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public OneofUpdate Clone()
    {
      return new OneofUpdate(this);
    }

    /// <summary>Field number for the "recipient_id" field.</summary>
    public const int RecipientIdFieldNumber = 1;
    private string recipientId_ = "";
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public string RecipientId
    {
      get { return recipientId_; }
      set
      {
        recipientId_ = pb::ProtoPreconditions.CheckNotNull(value, "value");
      }
    }

    /// <summary>Field number for the "game_id" field.</summary>
    public const int GameIdFieldNumber = 2;
    private string gameId_ = "";
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public string GameId
    {
      get { return gameId_; }
      set
      {
        gameId_ = pb::ProtoPreconditions.CheckNotNull(value, "value");
      }
    }

    /// <summary>Field number for the "all_soldier_positions" field.</summary>
    public const int AllSoldierPositionsFieldNumber = 3;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public global::Schema.AllSoldierPositions AllSoldierPositions
    {
      get { return updateCase_ == UpdateOneofCase.AllSoldierPositions ? (global::Schema.AllSoldierPositions)update_ : null; }
      set
      {
        update_ = value;
        updateCase_ = value == null ? UpdateOneofCase.None : UpdateOneofCase.AllSoldierPositions;
      }
    }

    private object update_;
    /// <summary>Enum of possible cases for the "update" oneof.</summary>
    public enum UpdateOneofCase
    {
      None = 0,
      AllSoldierPositions = 3,
    }
    private UpdateOneofCase updateCase_ = UpdateOneofCase.None;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public UpdateOneofCase UpdateCase
    {
      get { return updateCase_; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public void ClearUpdate()
    {
      updateCase_ = UpdateOneofCase.None;
      update_ = null;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public override bool Equals(object other)
    {
      return Equals(other as OneofUpdate);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public bool Equals(OneofUpdate other)
    {
      if (ReferenceEquals(other, null))
      {
        return false;
      }
      if (ReferenceEquals(other, this))
      {
        return true;
      }
      if (RecipientId != other.RecipientId) return false;
      if (GameId != other.GameId) return false;
      if (!object.Equals(AllSoldierPositions, other.AllSoldierPositions)) return false;
      if (UpdateCase != other.UpdateCase) return false;
      return Equals(_unknownFields, other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public override int GetHashCode()
    {
      int hash = 1;
      if (RecipientId.Length != 0) hash ^= RecipientId.GetHashCode();
      if (GameId.Length != 0) hash ^= GameId.GetHashCode();
      if (updateCase_ == UpdateOneofCase.AllSoldierPositions) hash ^= AllSoldierPositions.GetHashCode();
      hash ^= (int)updateCase_;
      if (_unknownFields != null)
      {
        hash ^= _unknownFields.GetHashCode();
      }
      return hash;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public override string ToString()
    {
      return pb::JsonFormatter.ToDiagnosticString(this);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public void WriteTo(pb::CodedOutputStream output)
    {
#if !GOOGLE_PROTOBUF_REFSTRUCT_COMPATIBILITY_MODE
      output.WriteRawMessage(this);
#else
      if (RecipientId.Length != 0) {
        output.WriteRawTag(10);
        output.WriteString(RecipientId);
      }
      if (GameId.Length != 0) {
        output.WriteRawTag(18);
        output.WriteString(GameId);
      }
      if (updateCase_ == UpdateOneofCase.AllSoldierPositions) {
        output.WriteRawTag(26);
        output.WriteMessage(AllSoldierPositions);
      }
      if (_unknownFields != null) {
        _unknownFields.WriteTo(output);
      }
#endif
    }

#if !GOOGLE_PROTOBUF_REFSTRUCT_COMPATIBILITY_MODE
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    void pb::IBufferMessage.InternalWriteTo(ref pb::WriteContext output)
    {
      if (RecipientId.Length != 0)
      {
        output.WriteRawTag(10);
        output.WriteString(RecipientId);
      }
      if (GameId.Length != 0)
      {
        output.WriteRawTag(18);
        output.WriteString(GameId);
      }
      if (updateCase_ == UpdateOneofCase.AllSoldierPositions)
      {
        output.WriteRawTag(26);
        output.WriteMessage(AllSoldierPositions);
      }
      if (_unknownFields != null)
      {
        _unknownFields.WriteTo(ref output);
      }
    }
#endif

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public int CalculateSize()
    {
      int size = 0;
      if (RecipientId.Length != 0)
      {
        size += 1 + pb::CodedOutputStream.ComputeStringSize(RecipientId);
      }
      if (GameId.Length != 0)
      {
        size += 1 + pb::CodedOutputStream.ComputeStringSize(GameId);
      }
      if (updateCase_ == UpdateOneofCase.AllSoldierPositions)
      {
        size += 1 + pb::CodedOutputStream.ComputeMessageSize(AllSoldierPositions);
      }
      if (_unknownFields != null)
      {
        size += _unknownFields.CalculateSize();
      }
      return size;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public void MergeFrom(OneofUpdate other)
    {
      if (other == null)
      {
        return;
      }
      if (other.RecipientId.Length != 0)
      {
        RecipientId = other.RecipientId;
      }
      if (other.GameId.Length != 0)
      {
        GameId = other.GameId;
      }
      switch (other.UpdateCase)
      {
        case UpdateOneofCase.AllSoldierPositions:
          if (AllSoldierPositions == null)
          {
            AllSoldierPositions = new global::Schema.AllSoldierPositions();
          }
          AllSoldierPositions.MergeFrom(other.AllSoldierPositions);
          break;
      }

      _unknownFields = pb::UnknownFieldSet.MergeFrom(_unknownFields, other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public void MergeFrom(pb::CodedInputStream input)
    {
#if !GOOGLE_PROTOBUF_REFSTRUCT_COMPATIBILITY_MODE
      input.ReadRawMessage(this);
#else
      uint tag;
      while ((tag = input.ReadTag()) != 0) {
        switch(tag) {
          default:
            _unknownFields = pb::UnknownFieldSet.MergeFieldFrom(_unknownFields, input);
            break;
          case 10: {
            RecipientId = input.ReadString();
            break;
          }
          case 18: {
            GameId = input.ReadString();
            break;
          }
          case 26: {
            global::Schema.AllSoldierPositions subBuilder = new global::Schema.AllSoldierPositions();
            if (updateCase_ == UpdateOneofCase.AllSoldierPositions) {
              subBuilder.MergeFrom(AllSoldierPositions);
            }
            input.ReadMessage(subBuilder);
            AllSoldierPositions = subBuilder;
            break;
          }
        }
      }
#endif
    }

#if !GOOGLE_PROTOBUF_REFSTRUCT_COMPATIBILITY_MODE
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    void pb::IBufferMessage.InternalMergeFrom(ref pb::ParseContext input)
    {
      uint tag;
      while ((tag = input.ReadTag()) != 0)
      {
        switch (tag)
        {
          default:
            _unknownFields = pb::UnknownFieldSet.MergeFieldFrom(_unknownFields, ref input);
            break;
          case 10:
            {
              RecipientId = input.ReadString();
              break;
            }
          case 18:
            {
              GameId = input.ReadString();
              break;
            }
          case 26:
            {
              global::Schema.AllSoldierPositions subBuilder = new global::Schema.AllSoldierPositions();
              if (updateCase_ == UpdateOneofCase.AllSoldierPositions)
              {
                subBuilder.MergeFrom(AllSoldierPositions);
              }
              input.ReadMessage(subBuilder);
              AllSoldierPositions = subBuilder;
              break;
            }
        }
      }
    }
#endif

  }

  public sealed partial class Vector2 : pb::IMessage<Vector2>
#if !GOOGLE_PROTOBUF_REFSTRUCT_COMPATIBILITY_MODE
      , pb::IBufferMessage
#endif
  {
    private static readonly pb::MessageParser<Vector2> _parser = new pb::MessageParser<Vector2>(() => new Vector2());
    private pb::UnknownFieldSet _unknownFields;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public static pb::MessageParser<Vector2> Parser { get { return _parser; } }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public static pbr::MessageDescriptor Descriptor
    {
      get { return global::Schema.SchemaReflection.Descriptor.MessageTypes[1]; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    pbr::MessageDescriptor pb::IMessage.Descriptor
    {
      get { return Descriptor; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public Vector2()
    {
      OnConstruction();
    }

    partial void OnConstruction();

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public Vector2(Vector2 other) : this()
    {
      x_ = other.x_;
      y_ = other.y_;
      _unknownFields = pb::UnknownFieldSet.Clone(other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public Vector2 Clone()
    {
      return new Vector2(this);
    }

    /// <summary>Field number for the "x" field.</summary>
    public const int XFieldNumber = 1;
    private float x_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public float X
    {
      get { return x_; }
      set
      {
        x_ = value;
      }
    }

    /// <summary>Field number for the "y" field.</summary>
    public const int YFieldNumber = 2;
    private float y_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public float Y
    {
      get { return y_; }
      set
      {
        y_ = value;
      }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public override bool Equals(object other)
    {
      return Equals(other as Vector2);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public bool Equals(Vector2 other)
    {
      if (ReferenceEquals(other, null))
      {
        return false;
      }
      if (ReferenceEquals(other, this))
      {
        return true;
      }
      if (!pbc::ProtobufEqualityComparers.BitwiseSingleEqualityComparer.Equals(X, other.X)) return false;
      if (!pbc::ProtobufEqualityComparers.BitwiseSingleEqualityComparer.Equals(Y, other.Y)) return false;
      return Equals(_unknownFields, other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public override int GetHashCode()
    {
      int hash = 1;
      if (X != 0F) hash ^= pbc::ProtobufEqualityComparers.BitwiseSingleEqualityComparer.GetHashCode(X);
      if (Y != 0F) hash ^= pbc::ProtobufEqualityComparers.BitwiseSingleEqualityComparer.GetHashCode(Y);
      if (_unknownFields != null)
      {
        hash ^= _unknownFields.GetHashCode();
      }
      return hash;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public override string ToString()
    {
      return pb::JsonFormatter.ToDiagnosticString(this);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public void WriteTo(pb::CodedOutputStream output)
    {
#if !GOOGLE_PROTOBUF_REFSTRUCT_COMPATIBILITY_MODE
      output.WriteRawMessage(this);
#else
      if (X != 0F) {
        output.WriteRawTag(13);
        output.WriteFloat(X);
      }
      if (Y != 0F) {
        output.WriteRawTag(21);
        output.WriteFloat(Y);
      }
      if (_unknownFields != null) {
        _unknownFields.WriteTo(output);
      }
#endif
    }

#if !GOOGLE_PROTOBUF_REFSTRUCT_COMPATIBILITY_MODE
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    void pb::IBufferMessage.InternalWriteTo(ref pb::WriteContext output)
    {
      if (X != 0F)
      {
        output.WriteRawTag(13);
        output.WriteFloat(X);
      }
      if (Y != 0F)
      {
        output.WriteRawTag(21);
        output.WriteFloat(Y);
      }
      if (_unknownFields != null)
      {
        _unknownFields.WriteTo(ref output);
      }
    }
#endif

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public int CalculateSize()
    {
      int size = 0;
      if (X != 0F)
      {
        size += 1 + 4;
      }
      if (Y != 0F)
      {
        size += 1 + 4;
      }
      if (_unknownFields != null)
      {
        size += _unknownFields.CalculateSize();
      }
      return size;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public void MergeFrom(Vector2 other)
    {
      if (other == null)
      {
        return;
      }
      if (other.X != 0F)
      {
        X = other.X;
      }
      if (other.Y != 0F)
      {
        Y = other.Y;
      }
      _unknownFields = pb::UnknownFieldSet.MergeFrom(_unknownFields, other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public void MergeFrom(pb::CodedInputStream input)
    {
#if !GOOGLE_PROTOBUF_REFSTRUCT_COMPATIBILITY_MODE
      input.ReadRawMessage(this);
#else
      uint tag;
      while ((tag = input.ReadTag()) != 0) {
        switch(tag) {
          default:
            _unknownFields = pb::UnknownFieldSet.MergeFieldFrom(_unknownFields, input);
            break;
          case 13: {
            X = input.ReadFloat();
            break;
          }
          case 21: {
            Y = input.ReadFloat();
            break;
          }
        }
      }
#endif
    }

#if !GOOGLE_PROTOBUF_REFSTRUCT_COMPATIBILITY_MODE
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    void pb::IBufferMessage.InternalMergeFrom(ref pb::ParseContext input)
    {
      uint tag;
      while ((tag = input.ReadTag()) != 0)
      {
        switch (tag)
        {
          default:
            _unknownFields = pb::UnknownFieldSet.MergeFieldFrom(_unknownFields, ref input);
            break;
          case 13:
            {
              X = input.ReadFloat();
              break;
            }
          case 21:
            {
              Y = input.ReadFloat();
              break;
            }
        }
      }
    }
#endif

  }

  public sealed partial class AllSoldierPositions : pb::IMessage<AllSoldierPositions>
#if !GOOGLE_PROTOBUF_REFSTRUCT_COMPATIBILITY_MODE
      , pb::IBufferMessage
#endif
  {
    private static readonly pb::MessageParser<AllSoldierPositions> _parser = new pb::MessageParser<AllSoldierPositions>(() => new AllSoldierPositions());
    private pb::UnknownFieldSet _unknownFields;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public static pb::MessageParser<AllSoldierPositions> Parser { get { return _parser; } }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public static pbr::MessageDescriptor Descriptor
    {
      get { return global::Schema.SchemaReflection.Descriptor.MessageTypes[2]; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    pbr::MessageDescriptor pb::IMessage.Descriptor
    {
      get { return Descriptor; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public AllSoldierPositions()
    {
      OnConstruction();
    }

    partial void OnConstruction();

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public AllSoldierPositions(AllSoldierPositions other) : this()
    {
      soldierPositions_ = other.soldierPositions_.Clone();
      _unknownFields = pb::UnknownFieldSet.Clone(other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public AllSoldierPositions Clone()
    {
      return new AllSoldierPositions(this);
    }

    /// <summary>Field number for the "soldier_positions" field.</summary>
    public const int SoldierPositionsFieldNumber = 1;
    private static readonly pb::FieldCodec<global::Schema.SoldierPosition> _repeated_soldierPositions_codec
        = pb::FieldCodec.ForMessage(10, global::Schema.SoldierPosition.Parser);
    private readonly pbc::RepeatedField<global::Schema.SoldierPosition> soldierPositions_ = new pbc::RepeatedField<global::Schema.SoldierPosition>();
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public pbc::RepeatedField<global::Schema.SoldierPosition> SoldierPositions
    {
      get { return soldierPositions_; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public override bool Equals(object other)
    {
      return Equals(other as AllSoldierPositions);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public bool Equals(AllSoldierPositions other)
    {
      if (ReferenceEquals(other, null))
      {
        return false;
      }
      if (ReferenceEquals(other, this))
      {
        return true;
      }
      if (!soldierPositions_.Equals(other.soldierPositions_)) return false;
      return Equals(_unknownFields, other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public override int GetHashCode()
    {
      int hash = 1;
      hash ^= soldierPositions_.GetHashCode();
      if (_unknownFields != null)
      {
        hash ^= _unknownFields.GetHashCode();
      }
      return hash;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public override string ToString()
    {
      return pb::JsonFormatter.ToDiagnosticString(this);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public void WriteTo(pb::CodedOutputStream output)
    {
#if !GOOGLE_PROTOBUF_REFSTRUCT_COMPATIBILITY_MODE
      output.WriteRawMessage(this);
#else
      soldierPositions_.WriteTo(output, _repeated_soldierPositions_codec);
      if (_unknownFields != null) {
        _unknownFields.WriteTo(output);
      }
#endif
    }

#if !GOOGLE_PROTOBUF_REFSTRUCT_COMPATIBILITY_MODE
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    void pb::IBufferMessage.InternalWriteTo(ref pb::WriteContext output)
    {
      soldierPositions_.WriteTo(ref output, _repeated_soldierPositions_codec);
      if (_unknownFields != null)
      {
        _unknownFields.WriteTo(ref output);
      }
    }
#endif

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public int CalculateSize()
    {
      int size = 0;
      size += soldierPositions_.CalculateSize(_repeated_soldierPositions_codec);
      if (_unknownFields != null)
      {
        size += _unknownFields.CalculateSize();
      }
      return size;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public void MergeFrom(AllSoldierPositions other)
    {
      if (other == null)
      {
        return;
      }
      soldierPositions_.Add(other.soldierPositions_);
      _unknownFields = pb::UnknownFieldSet.MergeFrom(_unknownFields, other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public void MergeFrom(pb::CodedInputStream input)
    {
#if !GOOGLE_PROTOBUF_REFSTRUCT_COMPATIBILITY_MODE
      input.ReadRawMessage(this);
#else
      uint tag;
      while ((tag = input.ReadTag()) != 0) {
        switch(tag) {
          default:
            _unknownFields = pb::UnknownFieldSet.MergeFieldFrom(_unknownFields, input);
            break;
          case 10: {
            soldierPositions_.AddEntriesFrom(input, _repeated_soldierPositions_codec);
            break;
          }
        }
      }
#endif
    }

#if !GOOGLE_PROTOBUF_REFSTRUCT_COMPATIBILITY_MODE
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    void pb::IBufferMessage.InternalMergeFrom(ref pb::ParseContext input)
    {
      uint tag;
      while ((tag = input.ReadTag()) != 0)
      {
        switch (tag)
        {
          default:
            _unknownFields = pb::UnknownFieldSet.MergeFieldFrom(_unknownFields, ref input);
            break;
          case 10:
            {
              soldierPositions_.AddEntriesFrom(ref input, _repeated_soldierPositions_codec);
              break;
            }
        }
      }
    }
#endif

  }

  public sealed partial class SoldierPosition : pb::IMessage<SoldierPosition>
#if !GOOGLE_PROTOBUF_REFSTRUCT_COMPATIBILITY_MODE
      , pb::IBufferMessage
#endif
  {
    private static readonly pb::MessageParser<SoldierPosition> _parser = new pb::MessageParser<SoldierPosition>(() => new SoldierPosition());
    private pb::UnknownFieldSet _unknownFields;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public static pb::MessageParser<SoldierPosition> Parser { get { return _parser; } }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public static pbr::MessageDescriptor Descriptor
    {
      get { return global::Schema.SchemaReflection.Descriptor.MessageTypes[3]; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    pbr::MessageDescriptor pb::IMessage.Descriptor
    {
      get { return Descriptor; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public SoldierPosition()
    {
      OnConstruction();
    }

    partial void OnConstruction();

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public SoldierPosition(SoldierPosition other) : this()
    {
      id_ = other.id_;
      pos_ = other.pos_ != null ? other.pos_.Clone() : null;
      velocity_ = other.velocity_ != null ? other.velocity_.Clone() : null;
      _unknownFields = pb::UnknownFieldSet.Clone(other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public SoldierPosition Clone()
    {
      return new SoldierPosition(this);
    }

    /// <summary>Field number for the "id" field.</summary>
    public const int IdFieldNumber = 1;
    private ulong id_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public ulong Id
    {
      get { return id_; }
      set
      {
        id_ = value;
      }
    }

    /// <summary>Field number for the "pos" field.</summary>
    public const int PosFieldNumber = 2;
    private global::Schema.Vector2 pos_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public global::Schema.Vector2 Pos
    {
      get { return pos_; }
      set
      {
        pos_ = value;
      }
    }

    /// <summary>Field number for the "velocity" field.</summary>
    public const int VelocityFieldNumber = 3;
    private global::Schema.Vector2 velocity_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public global::Schema.Vector2 Velocity
    {
      get { return velocity_; }
      set
      {
        velocity_ = value;
      }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public override bool Equals(object other)
    {
      return Equals(other as SoldierPosition);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public bool Equals(SoldierPosition other)
    {
      if (ReferenceEquals(other, null))
      {
        return false;
      }
      if (ReferenceEquals(other, this))
      {
        return true;
      }
      if (Id != other.Id) return false;
      if (!object.Equals(Pos, other.Pos)) return false;
      if (!object.Equals(Velocity, other.Velocity)) return false;
      return Equals(_unknownFields, other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public override int GetHashCode()
    {
      int hash = 1;
      if (Id != 0UL) hash ^= Id.GetHashCode();
      if (pos_ != null) hash ^= Pos.GetHashCode();
      if (velocity_ != null) hash ^= Velocity.GetHashCode();
      if (_unknownFields != null)
      {
        hash ^= _unknownFields.GetHashCode();
      }
      return hash;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public override string ToString()
    {
      return pb::JsonFormatter.ToDiagnosticString(this);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public void WriteTo(pb::CodedOutputStream output)
    {
#if !GOOGLE_PROTOBUF_REFSTRUCT_COMPATIBILITY_MODE
      output.WriteRawMessage(this);
#else
      if (Id != 0UL) {
        output.WriteRawTag(8);
        output.WriteUInt64(Id);
      }
      if (pos_ != null) {
        output.WriteRawTag(18);
        output.WriteMessage(Pos);
      }
      if (velocity_ != null) {
        output.WriteRawTag(26);
        output.WriteMessage(Velocity);
      }
      if (_unknownFields != null) {
        _unknownFields.WriteTo(output);
      }
#endif
    }

#if !GOOGLE_PROTOBUF_REFSTRUCT_COMPATIBILITY_MODE
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    void pb::IBufferMessage.InternalWriteTo(ref pb::WriteContext output)
    {
      if (Id != 0UL)
      {
        output.WriteRawTag(8);
        output.WriteUInt64(Id);
      }
      if (pos_ != null)
      {
        output.WriteRawTag(18);
        output.WriteMessage(Pos);
      }
      if (velocity_ != null)
      {
        output.WriteRawTag(26);
        output.WriteMessage(Velocity);
      }
      if (_unknownFields != null)
      {
        _unknownFields.WriteTo(ref output);
      }
    }
#endif

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public int CalculateSize()
    {
      int size = 0;
      if (Id != 0UL)
      {
        size += 1 + pb::CodedOutputStream.ComputeUInt64Size(Id);
      }
      if (pos_ != null)
      {
        size += 1 + pb::CodedOutputStream.ComputeMessageSize(Pos);
      }
      if (velocity_ != null)
      {
        size += 1 + pb::CodedOutputStream.ComputeMessageSize(Velocity);
      }
      if (_unknownFields != null)
      {
        size += _unknownFields.CalculateSize();
      }
      return size;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public void MergeFrom(SoldierPosition other)
    {
      if (other == null)
      {
        return;
      }
      if (other.Id != 0UL)
      {
        Id = other.Id;
      }
      if (other.pos_ != null)
      {
        if (pos_ == null)
        {
          Pos = new global::Schema.Vector2();
        }
        Pos.MergeFrom(other.Pos);
      }
      if (other.velocity_ != null)
      {
        if (velocity_ == null)
        {
          Velocity = new global::Schema.Vector2();
        }
        Velocity.MergeFrom(other.Velocity);
      }
      _unknownFields = pb::UnknownFieldSet.MergeFrom(_unknownFields, other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public void MergeFrom(pb::CodedInputStream input)
    {
#if !GOOGLE_PROTOBUF_REFSTRUCT_COMPATIBILITY_MODE
      input.ReadRawMessage(this);
#else
      uint tag;
      while ((tag = input.ReadTag()) != 0) {
        switch(tag) {
          default:
            _unknownFields = pb::UnknownFieldSet.MergeFieldFrom(_unknownFields, input);
            break;
          case 8: {
            Id = input.ReadUInt64();
            break;
          }
          case 18: {
            if (pos_ == null) {
              Pos = new global::Schema.Vector2();
            }
            input.ReadMessage(Pos);
            break;
          }
          case 26: {
            if (velocity_ == null) {
              Velocity = new global::Schema.Vector2();
            }
            input.ReadMessage(Velocity);
            break;
          }
        }
      }
#endif
    }

#if !GOOGLE_PROTOBUF_REFSTRUCT_COMPATIBILITY_MODE
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    void pb::IBufferMessage.InternalMergeFrom(ref pb::ParseContext input)
    {
      uint tag;
      while ((tag = input.ReadTag()) != 0)
      {
        switch (tag)
        {
          default:
            _unknownFields = pb::UnknownFieldSet.MergeFieldFrom(_unknownFields, ref input);
            break;
          case 8:
            {
              Id = input.ReadUInt64();
              break;
            }
          case 18:
            {
              if (pos_ == null)
              {
                Pos = new global::Schema.Vector2();
              }
              input.ReadMessage(Pos);
              break;
            }
          case 26:
            {
              if (velocity_ == null)
              {
                Velocity = new global::Schema.Vector2();
              }
              input.ReadMessage(Velocity);
              break;
            }
        }
      }
    }
#endif

  }

  #endregion

}

#endregion Designer generated code