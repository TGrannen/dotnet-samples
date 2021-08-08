import React, { useState, useEffect, useRef } from 'react';
import { HubConnectionBuilder } from '@microsoft/signalr';

const ChatInput = (props) => {
  const [user, setUser] = useState('');
  const [message, setMessage] = useState('');

  const onSubmit = (e) => {
    e.preventDefault();

    const isUserProvided = user && user !== '';
    const isMessageProvided = message && message !== '';

    if (isUserProvided && isMessageProvided) {
      props.sendMessage(user, message);
    } else {
      alert('Please insert an user and a message.');
    }
  };

  const onUserUpdate = (e) => {
    setUser(e.target.value);
  };

  const onMessageUpdate = (e) => {
    setMessage(e.target.value);
  };

  return (
    <form onSubmit={onSubmit}>
      <label htmlFor='user'>User:</label>
      <br />
      <input id='user' name='user' value={user} onChange={onUserUpdate} />
      <br />
      <label htmlFor='message'>Message:</label>
      <br />
      <input type='text' id='message' name='message' value={message} onChange={onMessageUpdate} />
      <br />
      <br />
      <button>Submit</button>
    </form>
  );
};

const Message = (props) => (
  <div style={{ background: '#eee', borderRadius: '5px', padding: '0 10px' }}>
    <p>
      <strong>{props.user}</strong> says:
    </p>
    <p>{props.message}</p>
  </div>
);

const ChatWindow = (props) => {
  const chat = props.chat.map((m) => <Message key={Date.now() * Math.random()} user={m.user} message={m.message} />);

  return <div>{chat}</div>;
};

const Chat = () => {
  const [connection, setConnection] = useState(null);
  const [chat, setChat] = useState([]);
  const latestChat = useRef(null);

  latestChat.current = chat;

  useEffect(() => {
    const newConnection = new HubConnectionBuilder().withUrl('/chathub').withAutomaticReconnect().build();

    setConnection(newConnection);
  }, []);

  useEffect(() => {
    if (connection) {
      connection
        .start()
        .then((result) => {
          console.log('Connected!');

          connection.on('ReceiveMessage', (user, message) => {
            const updatedChat = [...latestChat.current];
            const chatMessage = {
              user: user,
              message: message,
            };
            updatedChat.push(chatMessage);

            setChat(updatedChat);
          });
        })
        .catch((e) => console.log('Connection failed: ', e));
    }
  }, [connection]);

  const sendMessage = async (user, message) => {
    // const chatMessage = {
    //   user: user,
    //   message: message,
    // };

    if (connection.connectionStarted) {
      try {
        await connection.send('SendMessage', user, message);
      } catch (e) {
        console.log(e);
      }
    } else {
      alert('No connection to server yet.');
    }
  };

  return (
    <div>
      <ChatInput sendMessage={sendMessage} />
      <hr />
      <ChatWindow chat={chat} />
    </div>
  );
};

export default Chat;
