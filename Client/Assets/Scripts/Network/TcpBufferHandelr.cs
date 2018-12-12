using UnityEngine;
using System.Collections;
using System;

public class Defines {
    public static readonly int HEADERSIZE = 4;
}

public class TcpBufferHandelr {
     public delegate void CompletedMessageCallback(byte[] buffer);
    int messageSize;
    byte[] messageBuffer = new byte[1024];
    int current_position;
    int positionToRead;
    int remain_bytes;

    public TcpBufferHandelr() {
        this.messageSize = 0;
        this.current_position = 0;
        this.positionToRead = 0;
        this.remain_bytes = 0;
    }

    private bool Read(byte[] buffer, ref int src_position, int offset, int transffered) {
        if (this.current_position >= offset + transffered) {
            return false;
        }

        int copy_size = this.positionToRead - this.current_position;
        if (this.remain_bytes < copy_size) {
            copy_size = this.remain_bytes;
        }

        Array.Copy(buffer, src_position, this.messageBuffer, this.current_position, copy_size);
        src_position += copy_size;
        this.current_position += copy_size;
        this.remain_bytes -= copy_size;
        if (this.current_position < this.positionToRead) {
            return false;
        }
        return true;
    }


    public void OnRecevie(byte[] buffer, int offset, int transffered, CompletedMessageCallback callback) {
        this.remain_bytes = transffered;
        int src_position = offset;
        while (this.remain_bytes > 0) {
            bool completed = false;
            if (this.current_position < Defines.HEADERSIZE) {
                this.positionToRead = Defines.HEADERSIZE;
                completed = Read(buffer, ref src_position, offset, transffered);
                if (completed == false) {
                    return;
                }

                this.messageSize = GetBodySize();
                if (this.messageSize == 0) {
                    break;
                }
                this.positionToRead = this.messageSize + Defines.HEADERSIZE;
            }
            completed = Read(buffer, ref src_position, offset, transffered);

            if (completed) {
                callback(this.messageBuffer);
                ClearBuffer();
            }
        }
    }

    private int GetBodySize() {
        Type type = Defines.HEADERSIZE.GetType();
        if (type.Equals(typeof(Int16))) {
            return BitConverter.ToInt16(this.messageBuffer, 0);
        }

        return BitConverter.ToInt32(this.messageBuffer, 0);
    }

    private void ClearBuffer() {
        Array.Clear(this.messageBuffer, 0, this.messageBuffer.Length);
        this.current_position = 0;
        this.messageSize = 0;
    }
}
