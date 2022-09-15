using static ACL.ACL;

namespace AssetStudio
{
    public static class ACLExtensions
    {
        public static void Process(this ACLClip m_ACLClip, out float[] values, out float[] times) => DecompressAll(m_ACLClip.m_ClipData, out values, out times);
    }
}
