import React, { useState } from "react";

const AddEvent: React.FC = () => {
  const [eventName, setEventName] = useState("");
  const [eventStart, setEventStart] = useState("");
  const [eventEnd, setEventEnd] = useState("");
  const [message, setMessage] = useState("");

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setMessage("");
    const payload = {
      eventName,
      eventType: 1,
      eventStart,
      eventEnd,
    };
    const res = await fetch("/api/events", {
      method: "POST",
      headers: { "Content-Type": "application/json" },
      body: JSON.stringify(payload),
    });
    if (res.ok) {
      setMessage("Event added successfully!");
      setEventName("");
      setEventStart("");
      setEventEnd("");
    } else {
      setMessage("Failed to add event.");
    }
  };

  return (
    <div style={{ padding: 24, maxWidth: 400, margin: "0 auto" }}>
      <h2 style={{ color: "#efdddb" }}>Add New Event</h2>
      <form onSubmit={handleSubmit}>
        <div style={{ marginBottom: 16 }}>
          <label style={{ color: "#efdddb" }}>Event Name:</label>
          <input
            type="text"
            value={eventName}
            onChange={(e) => setEventName(e.target.value)}
            required
            style={{ width: "100%", padding: 8, marginTop: 4 }}
          />
        </div>
        <div style={{ marginBottom: 16 }}>
          <label style={{ color: "#efdddb" }}>Start Date:</label>
          <input
            type="date"
            value={eventStart}
            onChange={(e) => setEventStart(e.target.value)}
            required
            style={{ width: "100%", padding: 8, marginTop: 4 }}
          />
        </div>
        <div style={{ marginBottom: 16 }}>
          <label style={{ color: "#efdddb" }}>End Date:</label>
          <input
            type="date"
            value={eventEnd}
            onChange={(e) => setEventEnd(e.target.value)}
            required
            style={{ width: "100%", padding: 8, marginTop: 4 }}
          />
        </div>
        <button
          type="submit"
          style={{ padding: "8px 16px", background: "#82172e", color: "#efdddb", border: "none", borderRadius: 4 }}
        >
          Add Event
        </button>
      </form>
      {message && <div style={{ color: message.includes("success") ? "#4caf50" : "#f44336", marginTop: 16 }}>{message}</div>}
    </div>
  );
};

export default AddEvent;
