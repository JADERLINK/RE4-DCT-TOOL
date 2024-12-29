// RE4_DCT_TOOL (PS4/NS)
// by: JADERLINK
// youtube.com/@JADERLINK
// github.com/JADERLINK

#pragma endian little
#pragma pattern_limit 2147483647

struct Entry
{
    u64 unk;
    u32 here = $;
    u64 offset;

    u8 start @ (offset + here);
};

struct DCT {
    char Magic[4]; //DICT
    u32 Fixed1;
    u32 Fixed2;
    padding[4];
    u64 Value1;
    u32 Amount;
    Entry entry[Amount];
};

DCT dct @ 0;