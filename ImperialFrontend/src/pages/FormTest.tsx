import { CustomForm } from "./CustomForm";
import { CustomTextField } from "./CustomTextField";

import { useState } from "react";

import { Dayjs } from "dayjs";
import { CustomTimeAndDateSelector } from "./CustomTimeAndDateSelector";

const FormTest: React.FC = () => {
  const [eventStart, setEventStart] = useState<Dayjs | null>(null);
  const [eventEnd, setEventEnd] = useState<Dayjs | null>(null);

  return (
    <div style={{ padding: 32 }}>
      <CustomForm title="Add an Event" apiEndpoint="/api/events">
        <CustomTextField id="eventName" label="Event Name" required />
        <CustomTimeAndDateSelector
          id="eventStart"
          label="Start Date & Time"
          value={eventStart}
          onChange={setEventStart}
        />

        <CustomTimeAndDateSelector
          id="eventEnd"
          label="End Date & Time"
          value={eventEnd}
          onChange={setEventEnd}
        />
      </CustomForm>
    </div>
  );
};
export default FormTest;
