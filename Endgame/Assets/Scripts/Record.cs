using System;
using System.Collections.Generic;

public class Record
{
    private readonly RecordType type;

    public int offset = -1;

    public Record()
    {
        BinaryRecordType binaryRecordType = (BinaryRecordType)Attribute.GetCustomAttribute(GetType(), typeof(BinaryRecordType));
        if (binaryRecordType != null)
        {
            type = binaryRecordType.type;
            return;
        }
        foreach (RecordType value in Enum.GetValues(typeof(RecordType)))
        {
            if (value.ToString().Equals(GetType().Name, StringComparison.OrdinalIgnoreCase))
            {
                type = value;
                return;
            }
        }
        throw new InvalidOperationException("Could not automatically find type for " + GetType());
    }

    public Record(RecordType type)
    {
        this.type = type;
    }

    public virtual RecordType GetRecordType()
    {
        return type;
    }

    public virtual void PreProcess(Dictionary<int, Record> objects)
    {
    }

    public virtual void Process(Dictionary<int, Record> objects)
    {
    }

    public virtual void PostProcess(Dictionary<int, Record> objects)
    {
    }
}

public enum RecordType
{
    SerializedStreamHeader = 0,
    ClassWithId = 1,
    SystemClassWithMembers = 2,
    ClassWithMembers = 3,
    SystemClassWithMembersAndTypes = 4,
    ClassWithMembersAndTypes = 5,
    BinaryObjectString = 6,
    BinaryArray = 7,
    MemberPrimitiveTyped = 8,
    MemberReference = 9,
    ObjectNull = 10,
    MessageEnd = 11,
    BinaryLibrary = 12,
    ObjectNullMultiple256 = 13,
    ObjectNullMultiple = 14,
    ArraySinglePrimitive = 15,
    ArraySingleObject = 16,
    ArraySingleString = 17,
    MethodCall = 18,
    MethodReturn = 19
}
public class BinaryRecordType : Attribute
{
    public RecordType type;

    public BinaryRecordType(RecordType type)
    {
        this.type = type;
    }
}