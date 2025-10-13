namespace M3U8.NET.Constants;

public static class Tag
{
    // Essential M3U/HLS Tags
    public static readonly string EXTM3U = "#EXTM3U";
    public static readonly string EXTINF = "#EXTINF:";

    // Common Prefix
    public static readonly string EXTX = "#EXT-X";

    // Configuration and Control Tags
    public static readonly string EXTXVERSION = $"{EXTX}-VERSION:";
    public static readonly string EXTXTARGETDURATION = $"{EXTX}-TARGETDURATION:";
    public static readonly string EXTXMEDIASEQUENCE = $"{EXTX}-MEDIA-SEQUENCE:";
    public static readonly string EXTXDISCONTINUITYSEQUENCE = $"{EXTX}-DISCONTINUITY-SEQUENCE:";
    public static readonly string EXTXPLAYLISTTYPE = $"{EXTX}-PLAYLIST-TYPE:";
    public static readonly string EXTXPROGRAMDATETIME = $"{EXTX}-PROGRAM-DATE-TIME:";
    public static readonly string EXTXENDLIST = $"{EXTX}-ENDLIST";
    public static readonly string EXTXSTART = $"{EXTX}-START:";

    // Segment Tags
    public static readonly string EXTXDISCONTINUITY = $"{EXTX}-DISCONTINUITY"; 
    public static readonly string EXTXBYTERANGE = $"{EXTX}-BYTERANGE:";
    public static readonly string EXTXMAP = $"{EXTX}-MAP:";

    // Control Tags (DRM and Ad-Insertion)
    public static readonly string EXTXKEY = $"{EXTX}-KEY"; 
    public static readonly string EXTXCUEOUT = $"{EXTX}-CUE-OUT:";
    public static readonly string EXTXCUEOUTCONT = $"{EXTX}-CUE-OUT-CONT:";
    public static readonly string EXTXCUEIN = $"{EXTX}-CUE-IN";
    public static readonly string EXTXSESSIONDATA = $"{EXTX}-SESSION-DATA:";

    // Multiple/Variant Playlist Tags (Master Playlist)
    public static readonly string EXTXMEDIA = $"{EXTX}-MEDIA"; 
    public static readonly string EXTXSTREAMINF = $"{EXTX}-STREAM-INF"; 
    public static readonly string EXTXIFRAMESTREAMINF = $"{EXTX}-I-FRAME-STREAM-INF"; 
    public static readonly string EXTXIFRAMESONLY = $"{EXTX}-I-FRAMES-ONLY"; 
    public static readonly string EXTXINDEPENDENTSEGMENTS = $"{EXTX}-INDEPENDENT-SEGMENTS"; 
}