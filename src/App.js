import React, { useState } from 'react';
import axios from 'axios';
import './App.css';

function App() {
  const [messages, setMessages] = useState([
    { sender: 'bot', text: 'Hello! I am the TechNova Sales Assistant. How can I help you today?' }
  ]);
  const [input, setInput] = useState('');
  const [loading, setLoading] = useState(false);
  const [extracting, setExtracting] = useState(false);
  const [leadSaved, setLeadSaved] = useState(false);

  const sendMessage = async (e) => {
    e.preventDefault();
    if (!input.trim()) return;

    const userMessage = { sender: 'user', text: input };
    setMessages((prev) => [...prev, userMessage]);
    setInput('');
    setLoading(true);

    try {
      const response = await axios.post('https://localhost:7232/api/SalesAgent/chat', {
        message: input
      });

      const botMessage = { sender: 'bot', text: response.data.response };
      setMessages((prev) => [...prev, botMessage]);
    } catch (error) {
      console.error("Error:", error);
      setMessages((prev) => [...prev, { sender: 'bot', text: "Sorry, I couldn't connect to the server." }]);
    } finally {
      setLoading(false);
    }
  };

  const saveLead = async () => {
    if (messages.length <= 1) {
      alert("Please chat with the AI first before saving your info.");
      return;
    }

    setExtracting(true);
    
    // Format the whole chat history into one string for the AI to read
    const transcript = messages.map(m => `${m.sender === 'user' ? 'Customer' : 'AI'}: ${m.text}`).join('\n');

    try {
      const response = await axios.post('https://localhost:7232/api/SalesAgent/extract-lead', {
        message: transcript
      });

      console.log("Lead Extracted:", response.data);
      setLeadSaved(true);
      alert(`Success! The AI saved your info as a Lead in our database.\n\nName: ${response.data.lead.name}\nEmail: ${response.data.lead.email}`);
    } catch (error) {
      console.error("Error extracting lead:", error);
      alert("Failed to save lead. Check the console.");
    } finally {
      setExtracting(false);
    }
  };

  return (
    <div className="App" style={{ padding: '20px', maxWidth: '600px', margin: '0 auto', fontFamily: 'Arial' }}>
      
      {/* Header with the Save Lead button */}
      <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', marginBottom: '20px' }}>
        <h2 style={{ margin: 0, color: '#333' }}>TechNova Sales Agent</h2>
        <button 
          onClick={saveLead} 
          disabled={extracting || leadSaved}
          style={{ 
            padding: '10px 15px', 
            backgroundColor: leadSaved ? '#6c757d' : '#17a2b8', 
            color: 'white', 
            border: 'none', 
            borderRadius: '4px', 
            cursor: leadSaved ? 'not-allowed' : 'pointer' 
          }}
        >
          {extracting ? "Extracting..." : leadSaved ? "Lead Saved!" : "Save me as a Lead"}
        </button>
      </div>

      <div style={{ border: '1px solid #ddd', borderRadius: '8px', height: '400px', overflowY: 'scroll', padding: '15px', marginBottom: '20px', backgroundColor: '#f9f9f9' }}>
        {messages.map((msg, index) => (
          <div key={index} style={{ textAlign: msg.sender === 'user' ? 'right' : 'left', margin: '10px 0' }}>
            <span style={{
              display: 'inline-block',
              padding: '10px 15px',
              borderRadius: '20px',
              backgroundColor: msg.sender === 'user' ? '#007bff' : '#e9ecef',
              color: msg.sender === 'user' ? 'white' : 'black',
              maxWidth: '75%',
              textAlign: 'left'
            }}>
              {msg.text}
            </span>
          </div>
        ))}
        {loading && <div style={{ textAlign: 'left', fontStyle: 'italic', color: 'gray' }}>AI is typing...</div>}
      </div>

      <form onSubmit={sendMessage} style={{ display: 'flex' }}>
        <input
          type="text"
          value={input}
          onChange={(e) => setInput(e.target.value)}
          placeholder="Ask about our services..."
          style={{ flex: 1, padding: '12px', fontSize: '16px', borderRadius: '4px', border: '1px solid #ccc' }}
        />
        <button type="submit" style={{ padding: '12px 24px', fontSize: '16px', marginLeft: '10px', backgroundColor: '#28a745', color: 'white', border: 'none', borderRadius: '4px', cursor: 'pointer' }}>
          Send
        </button>
      </form>
    </div>
  );
}

export default App;