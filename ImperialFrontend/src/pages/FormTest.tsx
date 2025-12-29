import { CustomForm } from "./CustomForm";
import { CustomTextField } from "./CustomTextField";
import { CustomTimeAndDateSelector } from "./CustomTimeAndDateSelector";
import { GenericGuildMemberList } from "./GenericGuildMemberList";

const FormTest: React.FC = () => {
  return (
    <div style={{ padding: 32 }}>
      <CustomForm title="Add an Event" apiEndpoint="/api/events">
        <CustomTextField id="eventName" label="Event Name" required />
        <CustomTimeAndDateSelector id="eventStart" label="Start Date & Time" />
        <CustomTimeAndDateSelector id="eventEnd" label="End Date & Time" />
      </CustomForm>

      <GenericGuildMemberList apiEndpoint="/api/guildmembers" />
    </div>
  );
};
export default FormTest;
