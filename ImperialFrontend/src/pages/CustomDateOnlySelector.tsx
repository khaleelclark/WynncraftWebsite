import { DatePicker } from "@mui/x-date-pickers/DatePicker";
import dayjs, { Dayjs } from "dayjs";
import { useCustomFormContext } from "../CustomFormContext";

interface DateSelectorProps {
  label: string;
  id: string;
}

export const CustomDateOnlySelector = ({ label, id }: DateSelectorProps) => {
  const { register, formValues } = useCustomFormContext();

  const el = (
    <>
      <DatePicker
        label={label}
        value={dayjs(formValues[id])}
        onChange={(newValue: Dayjs | null) =>
          register(id, newValue ? newValue.format("YYYY-MM-DD") : "")
        }
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
