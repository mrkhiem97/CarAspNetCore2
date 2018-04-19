import React, { Component } from 'react';
import $ from 'jquery';

class ChatForm extends Component {
    constructor() {
        super();
        $(document).ready(() => {
            let hostUrl = 'localhost:51080';

            // Socket
            let socket;
            let elSocketCreate = $("#btnSocket");
            let elSocketMessage = $("#btnSocketMessage");
            let elSocketClose = $("#btnSocketClose");
            let elChatContent = $('#chatContent');

            // Web socket
            elSocketCreate.click(function () {
                let scheme = document.location.protocol === "https:" ? "wss" : "ws";
                // let port = document.location.port ? (":" + document.location.port) : "";


                let userId = $("#userId").val();
                let socketUrl = scheme + "://" + hostUrl + "/ws/" + userId;

                socket = new WebSocket(socketUrl);

                // Event: Socket opened
                socket.onopen = event => {
                    console.log(`Socket open at ${socketUrl}`);
                };

                // Event: Socket closed
                socket.onclose = event => {
                    console.log('Socket closed');
                };

                // Event: Socket error
                socket.onerror = event => {
                    console.error('Socket error');
                };

                // Event: Socket receive message
                socket.onmessage = event => {
                    let strContent = elChatContent.val() + '\n' + event.data;
                    elChatContent.val(strContent);
                    elChatContent.scrollTop(elChatContent[0].scrollHeight);

                    console.log(`Message: ${event.data}`);
                    console.log(event);
                };

                elSocketCreate[0].disabled = true;
                elSocketMessage[0].disabled = false;
                elSocketClose[0].disabled = false;
                console.info('Socket info:');
                console.info(socket);
            });

            // Send message over socket
            elSocketMessage.click(function () {
                if (socket.readyState === WebSocket.OPEN) {
                    let elMessage = $('#message');
                    socket.send(JSON.stringify({ message: elMessage.val() }));
                    elMessage.val('');
                }
                else {
                    console.error("Connection is closed");
                }

                // socket.send({ message: '俺のこと、愛してる？' });
            });

            // Send message over socket
            elSocketClose.click(function (e) {
                socket.close();

                elSocketCreate[0].disabled = false;
                elSocketClose[0].disabled = true;
                elSocketMessage[0].disabled = true;
            });
        });
    }

    render() {
        return (
            <form className="form-horizontal">
                <div className="form-group">
                    <label className="control-label col-sm-2">User Id: </label>
                    <div className="col-sm-10">
                        <input type="text" id="userId" name="userId" className="form-control form-control-sm" value="KhiemDM123456" />
                    </div>
                </div>
                <div className="form-group">
                    <label className="control-label col-sm-2">Message: </label>
                    <div className="col-sm-10">
                        <input type="text" id="message" name="message" className="form-control form-control-sm" value="予約ステータス(0:注文待ち、1:自動注文済み、2:手動注文済み、3:注文キャンセル)" />
                    </div>
                </div>
                <div className="form-group">
                    <label className="control-label col-sm-2">Chat content:</label>
                    <div className="col-sm-10">
                        <textarea className="form-control form-control-sm" id="chatContent" rows="5">
                        </textarea>
                    </div>
                </div>
                <div className="form-group">
                    <button id="btnSocket" type="button" className="btn btn-sm btn-primary">Create Web Socket</button>
                    <button id="btnSocketClose" type="button" className="btn btn-sm btn-primary" disabled>Close socket</button>
                </div>
                <div className="form-group">
                    <button id="btnSocketMessage" type="button" className="btn btn-sm btn-primary" disabled>Send socket message</button>
                </div>
            </form>
        );
    }
}

export default ChatForm;