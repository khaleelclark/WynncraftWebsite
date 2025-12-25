import { CustomForm } from "./CustomForm";
import { CustomTextField } from "./CustomTextField";
import { CustomDropdown } from "./CustomDropdown";
import { useState } from "react";
import { CustomDateOnlySelector } from "./CustomDateOnlySelector";
import dayjs, { Dayjs } from "dayjs";

//this array would have to get all ranks from ranks table in the database
// maybe have an apiendpoint prop and get the data from there
const rankOptions = [2, 3, 4, 5, 6, 7];

//needs join date

const FormTest: React.FC = () => {
  const [rank, setRank] = useState("");
  // const [eventStart, setEventStart] = useState<Dayjs | null>(dayjs());
  // const [eventEnd, setEventEnd] = useState<Dayjs | null>(null);
  const [joinDate, setJoinDate] = useState<Dayjs | null>(dayjs()); // default to today
  return (
    <div style={{ padding: 32 }}>
      <CustomForm title="Add a Guild Member" apiEndpoint="/api/guildmembers">
        <CustomTextField id="discordTag" label="Discord Tag" required />
        <CustomTextField id="mainUsername" label="Main Username" required />
        <CustomTextField id="uuid" label="Minecraft UUID" required />
        <CustomDropdown
          id="rankId"
          label="Rank"
          dropdownOptions={rankOptions}
          value={rank}
          onChange={setRank}
          required
        />
        {/* <CustomTextField id="joinDate" label="Join Date" required /> */}
        <CustomDateOnlySelector
          id="joinDate"
          label="Join Date"
          value={joinDate}
          onChange={setJoinDate}
        />
      </CustomForm>

      {/* <CustomForm title="Add an Event" apiEndpoint="/api/events">
        <CustomTextField id="eventName" label="Event Name" required />
        <CustomDateOnlySelector
          label="Start Date & Time"
          id="eventStart"
          value={eventStart}
          onChange={setEventStart}
          required
          mode="iso-utc"
        />
        <CustomDateOnlySelector
          label="End Date & Time"
          id="eventEnd"
          value={eventEnd}
          onChange={setEventEnd}
          required
          minDateTime={eventStart ?? undefined}
          error={!!eventStart && !!eventEnd && eventEnd.isBefore(eventStart)}
          helperText={
            !!eventStart && !!eventEnd && eventEnd.isBefore(eventStart)
              ? "End date must be after start date"
              : undefined
          }
          mode="iso-utc"
        />
      </CustomForm> */}
    </div>
  );
};
export default FormTest;
