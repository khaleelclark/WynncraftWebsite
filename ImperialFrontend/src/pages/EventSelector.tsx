import React, { useEffect, useState } from "react";

interface Event {
  eventId: number;
  eventName: string;
  eventStart: string;
  eventEnd: string;
}

interface Props {
  onSelect: (event: Event | null) => void;
}

const CUSTOM_EVENT: Event = {
  eventId: -2,
  eventName: "Custom",
  eventStart: "",
  eventEnd: "",
};

const ALL_TIME_EVENT: Event = {
  eventId: -1,
  eventName: "All Time",
  eventStart: "2000-01-01",
  eventEnd: "2100-01-01",
};

const EventSelector: React.FC<Props> = ({ onSelect }) => {
  const [events, setEvents] = useState<Event[]>([]);
  const [selectedId, setSelectedId] = useState<number>(-1);

  useEffect(() => {
    fetch("/api/events")
      .then((res) => res.json())
      .then((data) => {
        setEvents([CUSTOM_EVENT, ALL_TIME_EVENT, ...data]);
      });
  }, []);

  useEffect(() => {
    const selected =
      events.find((e) => e.eventId === selectedId) || ALL_TIME_EVENT;
    onSelect(selected);
  }, [selectedId, events, onSelect]);

  return (
    <div style={{ marginBottom: 16 }}>
      <label style={{ color: "#efdddb", marginRight: 8 }}>Type:</label>
      <select
        value={selectedId}
        onChange={(e) => setSelectedId(Number(e.target.value))}
        style={{
          background: "#511220",
          color: "#efdddb",
          border: "1px solid #bc511c",
          borderRadius: 4,
          padding: 4,
        }}
      >
        {events.map((ev) => (
          <option key={ev.eventId} value={ev.eventId}>
            {ev.eventName}
          </option>
        ))}
      </select>
    </div>
  );
};

export default EventSelector;
