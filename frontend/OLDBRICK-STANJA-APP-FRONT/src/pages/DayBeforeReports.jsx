import { useEffect, useState } from "react";
import DailyReportPreview from "../components/DailyReportPreview";
import { getDailyReportJustByDate } from "../api/helpers";
function DayBeforeReports() {
  const [datum, setDatum] = useState("");
  const [idNaloga, setIdNaloga] = useState(null);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    const run = async () => {
      const yesterday = new Date();
      yesterday.setDate(yesterday.getDate() - 1);

      const iso = yesterday.toISOString().slice(0, 10);

      setDatum(iso);

      const res = await getDailyReportJustByDate(iso);
      console.log("API RESPONSE:", res);

      setIdNaloga(res.idNaloga);
    };

    run()
      .catch((err) => {
        console.error("run() crashed:", err);
        setIdNaloga(null);
      })
      .finally(() => {
        setLoading(false);
      });
  }, []);

  return (
    <div className="pt-20 px-4">
      <h1 className="text-xl font-semibold text-white">PRETHODNI DAN</h1>
      <p className="text-white/70 mt-2">Ovde ide report za prethodni dan!</p>

      <DailyReportPreview idNaloga={idNaloga} datum={datum} />
    </div>
  );
}

export default DayBeforeReports;
