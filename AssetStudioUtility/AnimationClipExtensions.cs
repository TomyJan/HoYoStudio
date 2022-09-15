using System.IO;
using System.Linq;
using System.Text;

namespace AssetStudio
{
    public static class AnimationClipExtensions
    {
        public static string Convert(this AnimationClip animationClip, Game game)
        {
            var converter = AnimationClipConverter.Process(animationClip);
            animationClip.m_RotationCurves = converter.Rotations.Union(animationClip.m_RotationCurves).ToArray();
            animationClip.m_EulerCurves = converter.Eulers.Union(animationClip.m_EulerCurves).ToArray();
            animationClip.m_PositionCurves = converter.Translations.Union(animationClip.m_PositionCurves).ToArray();
            animationClip.m_ScaleCurves = converter.Scales.Union(animationClip.m_ScaleCurves).ToArray();
            animationClip.m_FloatCurves = converter.Floats.Union(animationClip.m_FloatCurves).ToArray();
            animationClip.m_PPtrCurves = converter.PPtrs.Union(animationClip.m_PPtrCurves).ToArray();
            return ConvertSerializedAnimationClip(animationClip, game);
        }

        public static string ConvertSerializedAnimationClip(AnimationClip animationClip, Game game)
        {
            var sb = new StringBuilder();
            using (var stringWriter = new StringWriter(sb))
            {
                YAMLWriter writer = new YAMLWriter();
                YAMLDocument doc = ExportYAMLDocument(animationClip);
                writer.AddDocument(doc);
                writer.Write(stringWriter);
                return sb.ToString();
            }
        }

        public static YAMLDocument ExportYAMLDocument(AnimationClip animationClip)
        {
            YAMLDocument document = new YAMLDocument();
            YAMLMappingNode root = document.CreateMappingRoot();
            root.Tag = ((int)ClassIDType.AnimationClip).ToString();
            root.Anchor = ((int)ClassIDType.AnimationClip * 100000).ToString();
            YAMLMappingNode node = (YAMLMappingNode)animationClip.ExportYAML();
            root.Add(ClassIDType.AnimationClip.ToString(), node);
            return document;
        }
    }
}
