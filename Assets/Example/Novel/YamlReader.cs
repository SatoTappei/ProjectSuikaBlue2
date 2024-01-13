using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using YamlDotNet.Serialization;
using System.IO;

namespace PSB.Novel
{
    public class SceneFlow
    {
        public SceneNode[] Sequence;
    }

    public class SceneNode
    {
        public string LineName;
        public string Line;
        public string[] Commands;
    }

    public struct LineContent
    {
        public string Name;
        public string Line;
    }
    
    public static class YamlReader
    {
        /// <summary>
        /// YAML��ǂݍ���Ńf�V���A���C�Y
        /// </summary>
        public static SceneFlow Deserialize(string address)
        {
            StreamReader sr = new(address, System.Text.Encoding.UTF8);
            IDeserializer d = new DeserializerBuilder().Build();
            SceneFlow sf = d.Deserialize<SceneFlow>(sr);

            return sf;
        }

        /// <summary>
        /// �䎌�̃f�[�^�݂̂𒊏o���ĕԂ�
        /// </summary>
        public static LineContent[] ToLineContents(this SceneFlow flow)
        {
            List<LineContent> contents = new();
            foreach (SceneNode node in flow.Sequence)
            {
                contents.Add(new LineContent() { Line = node.Line, Name = node.LineName });
            }

            return contents.ToArray();
        }

        /// <summary>
        /// �R�}���h�݂̂𒊏o���ĕԂ�
        /// </summary>
        public static IReadOnlyList<string[]> ToCommands(this SceneFlow flow)
        {
            List<string[]> commands = new();
            foreach (SceneNode node in flow.Sequence)
            {
                // �R�}���h�������ꍇ���A�C���f�b�N�X���Y���Ȃ��悤�ɋ�̔z���ǉ�����
                if (node.Commands == null) commands.Add(new string[] { });
                else commands.Add(node.Commands);
            }

            return commands;
        }
    }
}