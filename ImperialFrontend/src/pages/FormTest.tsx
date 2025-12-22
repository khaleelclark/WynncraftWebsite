import { CustomForm } from "./CustomForm";
import { CustomTextField } from "./CustomTextField";
import { CustomDropdown } from "./CustomDropdown";
import { useState } from "react";

//this array would have to get all ranks from ranks table in the database
const rankOptions = [
  "Vonamor",
  "Duma",
  "Minor Nobility",
  "Landed Gentry",
  "Gentry",
  "Imperial Citizen",
];

//needs join date // uuid

const FormTest: React.FC = () => {
  const [rank, setRank] = useState("");
  return (
    <div style={{ padding: 32 }}>
      <CustomForm title="Add a Guild Member">
        <CustomTextField id="discord-tag" label="Discord Tag" required />
        <CustomTextField id="main-username" label="Main Username" required />
        <CustomDropdown
          id="rank"
          label="Rank"
          dropdownOptions={rankOptions}
          value={rank}
          onChange={setRank}
          required
        />
      </CustomForm>
    </div>
  );
};

export default FormTest;
