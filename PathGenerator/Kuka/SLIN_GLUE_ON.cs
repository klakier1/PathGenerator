using System;
using System.Globalization;

namespace PathGenerator.Kuka
{
    class SLIN_GLUE_ON : SLIN
    {
        public SLIN_GLUE_ON(double speed, E6POS e6pos, FDAT fdat, LDAT ldat, bool cont = true) : base(speed, e6pos, fdat, ldat, cont)
        {
        }

        //TRIGGER WHEN PATH=0 DELAY=Equipment[GlueActData.SystemNo, 1].GunOnDelay DO GlueOnUp(1) PRIO=-1
        //SLIN XKLB_06_05 WITH $VEL=SVEL_CP( 0.2, , LL), $TOOL=STOOL2( FKLB_06_05), $BASE= SBASE( FKLB_06_05.BASE_NO),$IPO_MODE=SIPO_MODE( FKLB_06_05.IPO_FRAME), $LOAD=SLOAD( FKLB_06_05.TOOL_NO), $ACC=SACC_CP( LL), $APO=SAPO( LL), $ORI_TYPE=SORI_TYP( LL), $JERK=SJERK( LL) C_SPL
        //;ENDFOLD
        virtual protected string GetTrigger()
        {
            return "TRIGGER WHEN PATH=0 DELAY=Equipment[GlueActData.SystemNo, 1].GunOnDelay DO GlueOnUp(1) PRIO=-1";
        }

        protected override string GetFold()
        {
            //                     ;FOLD SLIN KLB_06_05 CONT Vel=0.2 m/s L GlueOn Distance=0 mm Gun=1 Check PrePressure=No Tool[1]:GUN_01 Base[10]:WorkBase;%{PE}%R 4.2.10,%MKUKATPGLUE,%CGLUEON,%VSLIN_SB,%P 1:SLIN_SB, 2:KLB_06_05, 3:C_SPL, 5:0.2, 7:L, 11:0, 14:1, 16:#NO
            string fold = String.Format(";FOLD SLIN {0} CONT Vel={1} m/s {4} GlueOn Distance=0 mm Gun=1 Check PrePressure=No Tool[{2}]:GUN_01 Base[{3}]:WorkBase;%{{PE}}%R 4.2.10,%MKUKATPGLUE,%CGLUEON,%VSLIN_SB,%P 1:SLIN_SB, 2:{0}, 3:C_SPL, 5:{1}, 7:{4}, 11:0, 14:1, 16:#NO",
                e6pos.Name.Substring(1),
                speed.ToString("F", CultureInfo.InvariantCulture),
                fdat.Tool_no,
                fdat.Base_no,
                ldat.Name.Substring(1));
            return fold;
        }

        public override string ToString()
        {
            return GetFold() + "\r\n" + GetTrigger() + "\r\n" + GetSlin() + "\r\n" + GetEndFold();
        }
    }
}
