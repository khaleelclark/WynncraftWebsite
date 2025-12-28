import { DatePicker } from "@mui/x-date-pickers/DatePicker";
import { Dayjs } from "dayjs";
import { useCustomFormContext } from "../CustomFormContext";

interface DateSelectorProps {
  value: Dayjs | null;
  onChange: (value: Dayjs | null) => void;
  label: string;
  id: string;
}

export const CustomDateOnlySelector = ({
  label,
  value,
  id,
  onChange,
}: DateSelectorProps) => {
  const { register } = useCustomFormContext();
  const handleChange = (newValue: Dayjs | null) => {
    // update local state in FormTest
    onChange(newValue);
    // register formatted value for the payload
    register(id, newValue ? newValue.format("YYYY-MM-DD") : "");
  };

  let el = (
    <>
      <DatePicker
        label={label}
        value={value}
        onChange={handleChange}
        closeOnSelect
        format="YYYY-MM-DD"
        slotProps={{
          textField: {
            name: id,
            fullWidth: true,
          },
        }}
      />
    </>
  );
  return el;
};
