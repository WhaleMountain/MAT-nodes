#pragma warning disable 618
#pragma warning disable 612
#pragma warning disable 414
#pragma warning disable 168

namespace MessagePack.Resolvers
{
    using System;
    using MessagePack;

    public class GeneratedResolver : global::MessagePack.IFormatterResolver
    {
        public static readonly global::MessagePack.IFormatterResolver Instance = new GeneratedResolver();

        GeneratedResolver()
        {

        }

        public global::MessagePack.Formatters.IMessagePackFormatter<T> GetFormatter<T>()
        {
            return FormatterCache<T>.formatter;
        }

        static class FormatterCache<T>
        {
            public static readonly global::MessagePack.Formatters.IMessagePackFormatter<T> formatter;

            static FormatterCache()
            {
                var f = GeneratedResolverGetFormatterHelper.GetFormatter(typeof(T));
                if (f != null)
                {
                    formatter = (global::MessagePack.Formatters.IMessagePackFormatter<T>)f;
                }
            }
        }
    }

    internal static class GeneratedResolverGetFormatterHelper
    {
        static readonly global::System.Collections.Generic.Dictionary<Type, int> lookup;

        static GeneratedResolverGetFormatterHelper()
        {
            lookup = new global::System.Collections.Generic.Dictionary<Type, int>(8)
            {
                {typeof(global::System.Collections.Generic.List<global::MATNet.MNPlayer>), 0 },
                {typeof(global::System.Collections.Generic.Dictionary<string, object>), 1 },
                {typeof(object[]), 2 },
                {typeof(global::MATNet.MNRoomData.Status), 3 },
                {typeof(global::MATNet.MNPlayer), 4 },
                {typeof(global::MATNet.MNRoomData), 5 },
                {typeof(global::MATNet.Plugins.WebSocketClient.DataContainer), 6 },
                {typeof(global::MATNet.Plugins.WebSocketLobbyManager.DataContainer), 7 },
            };
        }

        internal static object GetFormatter(Type t)
        {
            int key;
            if (!lookup.TryGetValue(t, out key)) return null;

            switch (key)
            {
                case 0: return new global::MessagePack.Formatters.ListFormatter<global::MATNet.MNPlayer>();
                case 1: return new global::MessagePack.Formatters.DictionaryFormatter<string, object>();
                case 2: return new global::MessagePack.Formatters.ArrayFormatter<object>();
                case 3: return new MessagePack.Formatters.MATNet.StatusFormatter();
                case 4: return new MessagePack.Formatters.MATNet.MNPlayerFormatter();
                case 5: return new MessagePack.Formatters.MATNet.MNRoomDataFormatter();
                case 6: return new MessagePack.Formatters.MATNet.Plugins.WebSocketClient_DataContainerFormatter();
                case 7: return new MessagePack.Formatters.MATNet.Plugins.WebSocketLobbyManager_DataContainerFormatter();
                default: return null;
            }
        }
    }
}

#pragma warning restore 168
#pragma warning restore 414
#pragma warning restore 618
#pragma warning restore 612

#pragma warning disable 618
#pragma warning disable 612
#pragma warning disable 414
#pragma warning disable 168

namespace MessagePack.Formatters.MATNet
{
    using System;
    using MessagePack;

    public sealed class StatusFormatter : global::MessagePack.Formatters.IMessagePackFormatter<global::MATNet.MNRoomData.Status>
    {
        public int Serialize(ref byte[] bytes, int offset, global::MATNet.MNRoomData.Status value, global::MessagePack.IFormatterResolver formatterResolver)
        {
            return MessagePackBinary.WriteInt32(ref bytes, offset, (Int32)value);
        }
        
        public global::MATNet.MNRoomData.Status Deserialize(byte[] bytes, int offset, global::MessagePack.IFormatterResolver formatterResolver, out int readSize)
        {
            return (global::MATNet.MNRoomData.Status)MessagePackBinary.ReadInt32(bytes, offset, out readSize);
        }
    }


}

#pragma warning restore 168
#pragma warning restore 414
#pragma warning restore 618
#pragma warning restore 612


#pragma warning disable 618
#pragma warning disable 612
#pragma warning disable 414
#pragma warning disable 168

namespace MessagePack.Formatters.MATNet
{
    using System;
    using MessagePack;


    public sealed class MNPlayerFormatter : global::MessagePack.Formatters.IMessagePackFormatter<global::MATNet.MNPlayer>
    {

        public int Serialize(ref byte[] bytes, int offset, global::MATNet.MNPlayer value, global::MessagePack.IFormatterResolver formatterResolver)
        {
            if (value == null)
            {
                return global::MessagePack.MessagePackBinary.WriteNil(ref bytes, offset);
            }
            
            var startOffset = offset;
            offset += global::MessagePack.MessagePackBinary.WriteFixedArrayHeaderUnsafe(ref bytes, offset, 5);
            offset += formatterResolver.GetFormatterWithVerify<string>().Serialize(ref bytes, offset, value.displayName, formatterResolver);
            offset += formatterResolver.GetFormatterWithVerify<string>().Serialize(ref bytes, offset, value.uuid, formatterResolver);
            offset += formatterResolver.GetFormatterWithVerify<string>().Serialize(ref bytes, offset, value.lanIP, formatterResolver);
            offset += formatterResolver.GetFormatterWithVerify<string>().Serialize(ref bytes, offset, value.wanIP, formatterResolver);
            offset += formatterResolver.GetFormatterWithVerify<string[]>().Serialize(ref bytes, offset, value.hostableMethods, formatterResolver);
            return offset - startOffset;
        }

        public global::MATNet.MNPlayer Deserialize(byte[] bytes, int offset, global::MessagePack.IFormatterResolver formatterResolver, out int readSize)
        {
            if (global::MessagePack.MessagePackBinary.IsNil(bytes, offset))
            {
                readSize = 1;
                return null;
            }

            var startOffset = offset;
            var length = global::MessagePack.MessagePackBinary.ReadArrayHeader(bytes, offset, out readSize);
            offset += readSize;

            var __displayName__ = default(string);
            var __uuid__ = default(string);
            var __lanIP__ = default(string);
            var __wanIP__ = default(string);
            var __hostableMethods__ = default(string[]);

            for (int i = 0; i < length; i++)
            {
                var key = i;

                switch (key)
                {
                    case 0:
                        __displayName__ = formatterResolver.GetFormatterWithVerify<string>().Deserialize(bytes, offset, formatterResolver, out readSize);
                        break;
                    case 1:
                        __uuid__ = formatterResolver.GetFormatterWithVerify<string>().Deserialize(bytes, offset, formatterResolver, out readSize);
                        break;
                    case 2:
                        __lanIP__ = formatterResolver.GetFormatterWithVerify<string>().Deserialize(bytes, offset, formatterResolver, out readSize);
                        break;
                    case 3:
                        __wanIP__ = formatterResolver.GetFormatterWithVerify<string>().Deserialize(bytes, offset, formatterResolver, out readSize);
                        break;
                    case 4:
                        __hostableMethods__ = formatterResolver.GetFormatterWithVerify<string[]>().Deserialize(bytes, offset, formatterResolver, out readSize);
                        break;
                    default:
                        readSize = global::MessagePack.MessagePackBinary.ReadNextBlock(bytes, offset);
                        break;
                }
                offset += readSize;
            }

            readSize = offset - startOffset;

            var ____result = new global::MATNet.MNPlayer();
            ____result.displayName = __displayName__;
            ____result.uuid = __uuid__;
            ____result.lanIP = __lanIP__;
            ____result.wanIP = __wanIP__;
            ____result.hostableMethods = __hostableMethods__;
            return ____result;
        }
    }


    public sealed class MNRoomDataFormatter : global::MessagePack.Formatters.IMessagePackFormatter<global::MATNet.MNRoomData>
    {

        public int Serialize(ref byte[] bytes, int offset, global::MATNet.MNRoomData value, global::MessagePack.IFormatterResolver formatterResolver)
        {
            if (value == null)
            {
                return global::MessagePack.MessagePackBinary.WriteNil(ref bytes, offset);
            }
            
            var startOffset = offset;
            offset += global::MessagePack.MessagePackBinary.WriteFixedArrayHeaderUnsafe(ref bytes, offset, 11);
            offset += formatterResolver.GetFormatterWithVerify<global::MATNet.MNRoomData.Status>().Serialize(ref bytes, offset, value.status, formatterResolver);
            offset += formatterResolver.GetFormatterWithVerify<global::MATNet.MNPlayer>().Serialize(ref bytes, offset, value.hostPlayer, formatterResolver);
            offset += MessagePackBinary.WriteInt32(ref bytes, offset, value.hostPortNum);
            offset += MessagePackBinary.WriteInt32(ref bytes, offset, value.maxPlayer);
            offset += formatterResolver.GetFormatterWithVerify<global::MATNet.MNPlayer>().Serialize(ref bytes, offset, value.adminPlayer, formatterResolver);
            offset += MessagePackBinary.WriteInt32(ref bytes, offset, value.roomID);
            offset += formatterResolver.GetFormatterWithVerify<string>().Serialize(ref bytes, offset, value.roomName, formatterResolver);
            offset += formatterResolver.GetFormatterWithVerify<global::System.Collections.Generic.List<global::MATNet.MNPlayer>>().Serialize(ref bytes, offset, value.players, formatterResolver);
            offset += formatterResolver.GetFormatterWithVerify<string>().Serialize(ref bytes, offset, value.clientMethod, formatterResolver);
            offset += formatterResolver.GetFormatterWithVerify<string>().Serialize(ref bytes, offset, value.serverMethod, formatterResolver);
            offset += formatterResolver.GetFormatterWithVerify<global::System.Collections.Generic.Dictionary<string, object>>().Serialize(ref bytes, offset, value.additionalDatas, formatterResolver);
            return offset - startOffset;
        }

        public global::MATNet.MNRoomData Deserialize(byte[] bytes, int offset, global::MessagePack.IFormatterResolver formatterResolver, out int readSize)
        {
            if (global::MessagePack.MessagePackBinary.IsNil(bytes, offset))
            {
                readSize = 1;
                return null;
            }

            var startOffset = offset;
            var length = global::MessagePack.MessagePackBinary.ReadArrayHeader(bytes, offset, out readSize);
            offset += readSize;

            var __status__ = default(global::MATNet.MNRoomData.Status);
            var __hostPlayer__ = default(global::MATNet.MNPlayer);
            var __hostPortNum__ = default(int);
            var __maxPlayer__ = default(int);
            var __adminPlayer__ = default(global::MATNet.MNPlayer);
            var __roomID__ = default(int);
            var __roomName__ = default(string);
            var __players__ = default(global::System.Collections.Generic.List<global::MATNet.MNPlayer>);
            var __clientMethod__ = default(string);
            var __serverMethod__ = default(string);
            var __additionalDatas__ = default(global::System.Collections.Generic.Dictionary<string, object>);

            for (int i = 0; i < length; i++)
            {
                var key = i;

                switch (key)
                {
                    case 0:
                        __status__ = formatterResolver.GetFormatterWithVerify<global::MATNet.MNRoomData.Status>().Deserialize(bytes, offset, formatterResolver, out readSize);
                        break;
                    case 1:
                        __hostPlayer__ = formatterResolver.GetFormatterWithVerify<global::MATNet.MNPlayer>().Deserialize(bytes, offset, formatterResolver, out readSize);
                        break;
                    case 2:
                        __hostPortNum__ = MessagePackBinary.ReadInt32(bytes, offset, out readSize);
                        break;
                    case 3:
                        __maxPlayer__ = MessagePackBinary.ReadInt32(bytes, offset, out readSize);
                        break;
                    case 4:
                        __adminPlayer__ = formatterResolver.GetFormatterWithVerify<global::MATNet.MNPlayer>().Deserialize(bytes, offset, formatterResolver, out readSize);
                        break;
                    case 5:
                        __roomID__ = MessagePackBinary.ReadInt32(bytes, offset, out readSize);
                        break;
                    case 6:
                        __roomName__ = formatterResolver.GetFormatterWithVerify<string>().Deserialize(bytes, offset, formatterResolver, out readSize);
                        break;
                    case 7:
                        __players__ = formatterResolver.GetFormatterWithVerify<global::System.Collections.Generic.List<global::MATNet.MNPlayer>>().Deserialize(bytes, offset, formatterResolver, out readSize);
                        break;
                    case 8:
                        __clientMethod__ = formatterResolver.GetFormatterWithVerify<string>().Deserialize(bytes, offset, formatterResolver, out readSize);
                        break;
                    case 9:
                        __serverMethod__ = formatterResolver.GetFormatterWithVerify<string>().Deserialize(bytes, offset, formatterResolver, out readSize);
                        break;
                    case 10:
                        __additionalDatas__ = formatterResolver.GetFormatterWithVerify<global::System.Collections.Generic.Dictionary<string, object>>().Deserialize(bytes, offset, formatterResolver, out readSize);
                        break;
                    default:
                        readSize = global::MessagePack.MessagePackBinary.ReadNextBlock(bytes, offset);
                        break;
                }
                offset += readSize;
            }

            readSize = offset - startOffset;

            var ____result = new global::MATNet.MNRoomData();
            ____result.status = __status__;
            ____result.hostPlayer = __hostPlayer__;
            ____result.hostPortNum = __hostPortNum__;
            ____result.maxPlayer = __maxPlayer__;
            ____result.adminPlayer = __adminPlayer__;
            ____result.roomID = __roomID__;
            ____result.roomName = __roomName__;
            ____result.players = __players__;
            ____result.clientMethod = __clientMethod__;
            ____result.serverMethod = __serverMethod__;
            ____result.additionalDatas = __additionalDatas__;
            return ____result;
        }
    }

}

#pragma warning restore 168
#pragma warning restore 414
#pragma warning restore 618
#pragma warning restore 612
#pragma warning disable 618
#pragma warning disable 612
#pragma warning disable 414
#pragma warning disable 168

namespace MessagePack.Formatters.MATNet.Plugins
{
    using System;
    using MessagePack;


    public sealed class WebSocketClient_DataContainerFormatter : global::MessagePack.Formatters.IMessagePackFormatter<global::MATNet.Plugins.WebSocketClient.DataContainer>
    {

        public int Serialize(ref byte[] bytes, int offset, global::MATNet.Plugins.WebSocketClient.DataContainer value, global::MessagePack.IFormatterResolver formatterResolver)
        {
            if (value == null)
            {
                return global::MessagePack.MessagePackBinary.WriteNil(ref bytes, offset);
            }
            
            var startOffset = offset;
            offset += global::MessagePack.MessagePackBinary.WriteFixedArrayHeaderUnsafe(ref bytes, offset, 2);
            offset += formatterResolver.GetFormatterWithVerify<string>().Serialize(ref bytes, offset, value.id, formatterResolver);
            offset += formatterResolver.GetFormatterWithVerify<object[]>().Serialize(ref bytes, offset, value.data, formatterResolver);
            return offset - startOffset;
        }

        public global::MATNet.Plugins.WebSocketClient.DataContainer Deserialize(byte[] bytes, int offset, global::MessagePack.IFormatterResolver formatterResolver, out int readSize)
        {
            if (global::MessagePack.MessagePackBinary.IsNil(bytes, offset))
            {
                readSize = 1;
                return null;
            }

            var startOffset = offset;
            var length = global::MessagePack.MessagePackBinary.ReadArrayHeader(bytes, offset, out readSize);
            offset += readSize;

            var __id__ = default(string);
            var __data__ = default(object[]);

            for (int i = 0; i < length; i++)
            {
                var key = i;

                switch (key)
                {
                    case 0:
                        __id__ = formatterResolver.GetFormatterWithVerify<string>().Deserialize(bytes, offset, formatterResolver, out readSize);
                        break;
                    case 1:
                        __data__ = formatterResolver.GetFormatterWithVerify<object[]>().Deserialize(bytes, offset, formatterResolver, out readSize);
                        break;
                    default:
                        readSize = global::MessagePack.MessagePackBinary.ReadNextBlock(bytes, offset);
                        break;
                }
                offset += readSize;
            }

            readSize = offset - startOffset;

            var ____result = new global::MATNet.Plugins.WebSocketClient.DataContainer();
            ____result.id = __id__;
            ____result.data = __data__;
            return ____result;
        }
    }


    public sealed class WebSocketLobbyManager_DataContainerFormatter : global::MessagePack.Formatters.IMessagePackFormatter<global::MATNet.Plugins.WebSocketLobbyManager.DataContainer>
    {

        public int Serialize(ref byte[] bytes, int offset, global::MATNet.Plugins.WebSocketLobbyManager.DataContainer value, global::MessagePack.IFormatterResolver formatterResolver)
        {
            if (value == null)
            {
                return global::MessagePack.MessagePackBinary.WriteNil(ref bytes, offset);
            }
            
            var startOffset = offset;
            offset += global::MessagePack.MessagePackBinary.WriteFixedArrayHeaderUnsafe(ref bytes, offset, 2);
            offset += MessagePackBinary.WriteInt32(ref bytes, offset, value.roomID);
            offset += formatterResolver.GetFormatterWithVerify<global::MATNet.MNRoomData>().Serialize(ref bytes, offset, value.data, formatterResolver);
            return offset - startOffset;
        }

        public global::MATNet.Plugins.WebSocketLobbyManager.DataContainer Deserialize(byte[] bytes, int offset, global::MessagePack.IFormatterResolver formatterResolver, out int readSize)
        {
            if (global::MessagePack.MessagePackBinary.IsNil(bytes, offset))
            {
                readSize = 1;
                return null;
            }

            var startOffset = offset;
            var length = global::MessagePack.MessagePackBinary.ReadArrayHeader(bytes, offset, out readSize);
            offset += readSize;

            var __roomID__ = default(int);
            var __data__ = default(global::MATNet.MNRoomData);

            for (int i = 0; i < length; i++)
            {
                var key = i;

                switch (key)
                {
                    case 0:
                        __roomID__ = MessagePackBinary.ReadInt32(bytes, offset, out readSize);
                        break;
                    case 1:
                        __data__ = formatterResolver.GetFormatterWithVerify<global::MATNet.MNRoomData>().Deserialize(bytes, offset, formatterResolver, out readSize);
                        break;
                    default:
                        readSize = global::MessagePack.MessagePackBinary.ReadNextBlock(bytes, offset);
                        break;
                }
                offset += readSize;
            }

            readSize = offset - startOffset;

            var ____result = new global::MATNet.Plugins.WebSocketLobbyManager.DataContainer();
            ____result.roomID = __roomID__;
            ____result.data = __data__;
            return ____result;
        }
    }

}

#pragma warning restore 168
#pragma warning restore 414
#pragma warning restore 618
#pragma warning restore 612
