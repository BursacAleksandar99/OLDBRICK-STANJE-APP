import { useState } from "react";
import DailyReportPreview from "../components/DailyReportPreview";
import Calendar from "../components/Calendar";
import { createNalogByDate, getNalogByDate } from "../api/helpers";
import SaveDailyReportStates from "../components/SaveDailyReportStates";

function DailyReports() {
  const [datum, setDatum] = useState("");
  const [idNaloga, setIdNaloga] = useState(null);
  const [statusMessageForNalog, setStatusMessageForNalog] = useState("");

  async function handleGetOrCreateNalog(datum) {
    try {
      const existing = await getNalogByDate(datum);

      if (existing) {
        return;
      }

      const res = await createNalogByDate(datum);
      setIdNaloga(res.idNaloga);
    } catch (err) {
      console.error(err);
    }
  }
  async function handleCalendarChange(datum) {
    setDatum(datum);
    handleGetOrCreateNalog(datum);
  }

  console.log("URL", import.meta.env.VITE_API_BASE_URL);

  return (
    <div className="pt-20 px-4">
      <h1 className="text-xl font-semibold text-white">KREIRAJ DNEVNO NALOG</h1>
      <div className="pt-20 px-4">
        <div className="mt-4">
          <Calendar value={datum} onChange={handleCalendarChange} />

          {statusMessageForNalog && (
            <div className="fixed inset-0 z-50 flex items-center justify-center">
              {/* BACKDROP */}
              <div className="absolute inset-0 bg-black/50 backdrop-blur-sm" />

              {/* MESSAGE WINDOW */}
              <div
                className="relative z-10 rounded-xl bg-[#1f2937]
                 px-6 py-4 shadow-2xl
                 text-center max-w-sm w-[90%]"
              >
                <div className="text-white text-md">
                  {statusMessageForNalog}
                </div>
              </div>
            </div>
          )}
        </div>

        <SaveDailyReportStates idNaloga={idNaloga} />
      </div>
      <DailyReportPreview datum={datum} onidNalogaResolved={setIdNaloga} />
    </div>
  );
}

export default DailyReports;
