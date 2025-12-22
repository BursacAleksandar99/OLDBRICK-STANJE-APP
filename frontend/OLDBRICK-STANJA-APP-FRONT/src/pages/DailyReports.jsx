import { useState } from "react";
import DailyReportPreview from "../components/DailyReportPreview";
import Calendar from "../components/Calendar";
import ProsutoKantaForm from "../components/ProsutoKantaForm";

function DailyReports() {
  const [datum, setDatum] = useState("");
  const [idNaloga, setIdNaloga] = useState(null);

  return (
    <div className="pt-20 px-4">
      <h1 className="text-xl font-semibold text-white">Unesi stanje</h1>
      <div className="pt-20 px-4">
        <div className="mt-4">
          <Calendar value={datum} onChange={setDatum} />
        </div>
        <ProsutoKantaForm idNaloga={idNaloga} />

        <p className="mt-3 text-center text-white/60 text-sm">
          Izabrani datum: <span className="text-white">{datum || "-"}</span>
        </p>
      </div>
      <DailyReportPreview datum={datum} onidNalogaResolved={setIdNaloga} />
    </div>
  );
}

export default DailyReports;
