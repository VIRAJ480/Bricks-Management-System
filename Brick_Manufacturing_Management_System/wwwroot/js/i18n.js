/**
 * ══════════════════════════════════════════════════════════════
 *  i18n.js  —  Language Translation Engine
 *  Supports: English (en) | Marathi (mr)
 *
 *  HOW IT WORKS:
 *  1. Every piece of text in the HTML has a  data-i18n="key"  attribute.
 *  2. This file holds a dictionary of all those keys in both languages.
 *  3. When the user picks a language from the dropdown, applyLanguage()
 *     loops through every element that has data-i18n and swaps its text.
 *  4. The chosen language is saved in localStorage so it persists when
 *     the user navigates to another page.
 * ══════════════════════════════════════════════════════════════
 */

// ── TRANSLATION DICTIONARY ──────────────────────────────────
// Add a new key here whenever you add a new data-i18n="key" in any view.
// Format:  "key": { en: "English text", mr: "मराठी मजकूर" }
const TRANSLATIONS = {

    // ── TOPBAR ──────────────────────────────────────────────
    "brand.name": { en: "ज्योतिर्लिंग विट सप्लायर्स", mr: "ज्योतिर्लिंग विट सप्लायर्स" },
    "brand.sub": { en: "कै. तानाजीराव माने वीट उद्योग समूह", mr: "कै. तानाजीराव माने वीट उद्योग समूह" },
    "topbar.activeLabour":  { en: "Active Labour",          mr: "सक्रिय कामगार" },
    "topbar.todayBricks":   { en: "Today's Bricks",         mr: "आजच्या विटा" },
    "topbar.pendingAmt":    { en: "Pending Amt",            mr: "बाकी रक्कम" },
    "topbar.logout":        { en: "↩ Logout",               mr: "↩ बाहेर पडा" },
    "logout.title":         { en: "Log out?",               mr: "बाहेर पडायचे?" },
    "logout.message":       { en: "Are you sure you want to log out of your account?", mr: "तुम्हाला खात्यातून बाहेर पडायचे आहे का?" },
    "logout.no":            { en: "No",                     mr: "नाही" },
    "logout.yes":           { en: "Yes, Logout",            mr: "होय, बाहेर पडा" },
    "lang.label":           { en: "🌐 Language",            mr: "🌐 भाषा" },

    // ── SIDEBAR GROUP LABELS ─────────────────────────────────
    "nav.overview":         { en: "Overview",               mr: "आढावा" },
    "nav.vendorMat":        { en: "Vendors & Materials",    mr: "विक्रेते आणि साहित्य" },
    "nav.labour":           { en: "Labour Management",      mr: "कामगार व्यवस्थापन" },
    "nav.production":       { en: "Production",             mr: "उत्पादन" },
    "nav.sales":            { en: "Sales & Payments",       mr: "विक्री आणि देयके" },
    "nav.reports":          { en: "Reports",                mr: "अहवाल" },
    "nav.system":           { en: "System",                 mr: "प्रणाली" },
    "nav.grpMasters":       { en: "Masters",                mr: "मास्टर्स" },
    "nav.grpLabour":        { en: "Labour",                 mr: "कामगार" },
    "nav.grpMatVendorPay":  { en: "Material & Vendor Payment", mr: "साहित्य व विक्रेता देयक" },
    "nav.grpBricks":        { en: "Bricks",                 mr: "विटा" },
    "nav.grpSalesCustPay":  { en: "Brick Sales & Customer Payment", mr: "विटा विक्री व ग्राहक देयक" },

    // ── SIDEBAR NAV ITEMS ────────────────────────────────────
    "nav.dashboard":        { en: "Dashboard",              mr: "डॅशबोर्ड" },
    "nav.vendorMaster":     { en: "Vendor Master",          mr: "विक्रेता मास्टर" },
    "nav.materialMaster":   { en: "Material Master",        mr: "साहित्य मास्टर" },
    "nav.materialPurchase": { en: "Material Purchase",      mr: "साहित्य खरेदी" },
    "nav.labourMaster":     { en: "Labour Master",          mr: "कामगार मास्टर" },
    "nav.labourAdvance": { en: "Labour Advance", mr: "लेबर अॅडव्हान्स" },
    "nav.workEntry":        { en: "Work Entry",             mr: "काम नोंद" },
    "nav.labourExpense":    { en: "Labour Expense",         mr: "कामगार खर्च" },
    "nav.advDeduction": { en: "Advance Deduction", mr: "अॅडव्हान्स डिडक्शन" },
    "nav.salaryPayment": { en: "Salary Payment", mr: "सॅलरी पेमेंट" },
    "nav.labourLedger":     { en: "Labour Ledger Report",   mr: "कामगार खातेवही अहवाल" },
    "nav.brickProduction":  { en: "Brick Production",       mr: "विटा उत्पादन" },
    "nav.customerMaster": { en: "Customer Master", mr: "कस्टमर मास्टर" },
    "nav.brickSales":       { en: "Brick Sales",            mr: "विटा विक्री" },
    "nav.customerPayment": { en: "Customer Payment", mr: "कस्टमर पेमेंट्स" },
    "nav.materialReport":{ en: "Material Purchase Report", mr: "साहित्य खरेदी अहवाल" },
    "nav.customerSalesReport": { en: "Customer Sales Report", mr: "ग्राहक विक्री अहवाल" },
    "nav.customerSpecificReport": { en: "Customer Specific Report", mr: "ग्राहकनिहाय अहवाल" },
    "nav.vendorSpecificReport": { en: "Vendor Specific Purchase Report", mr: "विक्रेतानिहाय खरेदी अहवाल" },
    "nav.labourReport":     { en: "Labour Report",          mr: "कामगार अहवाल" },
    "nav.productionReport": { en: "Production Report",      mr: "उत्पादन अहवाल" },
    "nav.salesReport":      { en: "Sales Report",           mr: "विक्री अहवाल" },
    "nav.pendingPayments":  { en: "Customer Pending Payments",       mr: "प्रलंबित देयके" },
    "nav.accountSettings": { en: "Account Settings", mr: "खाते सेटिंग्ज" },
    "nav.VendorPayment": { en: "Vendor Payment", mr: "व्हेंडर पेमेंट" },
    "nav.BricksType":    { en: "Bricks Type",    mr: "विटांचा प्रकार" },
   

    // ── DASHBOARD — WELCOME BANNER ───────────────────────────
    "dash.overview":        { en: "Here's your Brick Factory overview",  mr: "येथे आपल्या विटा कारखान्याचा आढावा आहे" },
    "dash.factoryActive":   { en: "Factory Active",         mr: "कारखाना सुरू" },

    // ── DASHBOARD — STAT CARDS ───────────────────────────────
    "dash.vendors":         { en: "Vendors",                mr: "विक्रेते" },
    "dash.manage":          { en: "Manage →",               mr: "व्यवस्थापित करा →" },
    "dash.materialTypes":   { en: "Material Types",         mr: "साहित्य प्रकार" },
    "dash.viewAll":         { en: "View All →",             mr: "सर्व पहा →" },
    "dash.activeLabour":    { en: "Active Labour",          mr: "सक्रिय कामगार" },
    "dash.bricksToday":     { en: "Bricks Today",           mr: "आजच्या विटा" },
    "dash.production":      { en: "Production →",           mr: "उत्पादन →" },
    "dash.customers":       { en: "Customers",              mr: "ग्राहक" },
    "dash.salesMonth":      { en: "Sales This Month",       mr: "या महिन्यातील विक्री" },
    "dash.viewSales":       { en: "View Sales →",           mr: "विक्री पहा →" },
    "dash.revenueMonth":    { en: "Revenue (This Month)",   mr: "महसूल (या महिन्यात)" },
    "dash.report":          { en: "Report →",               mr: "अहवाल →" },
    "dash.pendingPayments": { en: "Customer Pending Payments",       mr: "प्रलंबित देयके" },
    "dash.review":          { en: "Review →",               mr: "तपासा →" },
    "dash.LabourLedgerReport": { en: "Labour Ledger Report", mr: "कामगार खातेवही अहवाल" },
    "dash.AdvanceDeduction": { en: "Advance Deduction", mr: "अॅडव्हान्स डिडक्शन" },
    "dash.Breadbricks":        { en: "Bread Bricks",         mr: "ब्रेड विटा" },
    "dash.Solidbricks": { en: "Solid Bricks", mr: "ठोकळा विटा" },
    "dash.Brokenbricks": { en: "Broken bricks", mr: "तुकडा विट" },
    "nav.VendorPendingPaymentReport": { en: "Vendor Pending Payments", mr: "व्हेंडर पेंडिंग पेमेंट्स" },
    "dash.VendorPendingPaymentReport": { en: "Vendor Pending Payments", mr: "व्हेंडर पेंडिंग पेमेंट्स" },

    // ── DASHBOARD — RECENT BRICK SALES TABLE ─────────────────
    "dash.recentSales":     { en: "Recent Brick Sales",     mr: "अलीकडील विटा विक्री" },
    "dash.col.customer":    { en: "Customer",               mr: "ग्राहक" },
    "dash.col.bricksQty":   { en: "Bricks (Qty)",           mr: "विटा (संख्या)" },
    "dash.col.rate":        { en: "Rate",                   mr: "दर" },
    "dash.col.amount":      { en: "Amount",                 mr: "रक्कम" },
    "dash.col.status":      { en: "Status",                 mr: "स्थिती" },
    "dash.badge.paid":      { en: "Paid",                   mr: "भरले" },
    "dash.badge.partial":   { en: "Partial",                mr: "अंशतः" },
    "dash.badge.pending":   { en: "Pending",                mr: "प्रलंबित" },

    // ── DASHBOARD — RECENT MATERIAL PURCHASES TABLE ──────────
    "dash.recentPurchases": { en: "Recent Material Purchases", mr: "अलीकडील साहित्य खरेदी" },
    "dash.col.vendor":      { en: "Vendor",                 mr: "विक्रेता" },
    "dash.col.material":    { en: "Material",               mr: "साहित्य" },
    "dash.col.qty":         { en: "Qty",                    mr: "प्रमाण" },
    "dash.badge.due":       { en: "Due",                    mr: "देय" },

    // ── DASHBOARD — LABOUR OVERVIEW ──────────────────────────
    "dash.labourOverview":  { en: "Labour Overview",        mr: "कामगार आढावा" },
    "dash.badge.active":    { en: "Active",                 mr: "सक्रिय" },
    "dash.badge.onLeave":   { en: "On Leave",               mr: "रजेवर" },

    // ── DASHBOARD — ADVANCE SUMMARY ──────────────────────────
    "dash.advanceSummary":  { en: "Advance Summary",        mr: "आगाऊ सारांश" },
    "dash.totalAdvances":   { en: "Total Advances Given",   mr: "एकूण दिलेले आगाऊ" },
    "dash.thisMonth":       { en: "This month",             mr: "या महिन्यात" },
    "dash.deductionsDone":  { en: "Deductions Done",        mr: "कपात झाली" },
    "dash.recovered":       { en: "Recovered",              mr: "वसूल" },
    "dash.outstanding":     { en: "Outstanding Balance",    mr: "थकीत शिल्लक" },
    "dash.pendingRecovery": { en: "Pending recovery",       mr: "वसुली प्रलंबित" },

    // ── DASHBOARD — PRODUCTION TABLE ─────────────────────────
    "dash.productionWeek":  { en: "Production (This Week)", mr: "उत्पादन (या आठवड्यात)" },
    "dash.col.date":        { en: "Date",                   mr: "तारीख" },
    "dash.col.rawBricks":   { en: "Raw Bricks",             mr: "कच्च्या विटा" },
    "dash.col.fired":       { en: "Fired",                  mr: "भाजलेल्या" },
    "dash.col.rejected":    { en: "Rejected",               mr: "नाकारलेल्या" },
    "dash.today":           { en: "Today",                  mr: "आज" },
    "dash.yesterday":       { en: "Yesterday",              mr: "काल" },

    // ── DASHBOARD — MATERIAL STOCK ───────────────────────────
    "dash.materialStock":   { en: "Material Stock",         mr: "साहित्य साठा" },
    "dash.availableStock":  { en: "Available stock",        mr: "उपलब्ध साठा" },

    // ── DASHBOARD — PENDING CUSTOMER DUES ───────────────────
    "dash.pendingDues":     { en: "Pending Customer Dues",  mr: "प्रलंबित ग्राहक देणी" },
    "dash.days":            { en: "days",                   mr: "दिवस" },
    "dash.badge.unpaid":    { en: "Unpaid",                 mr: "न भरलेले" },

    // ── MATERIAL REPORT ──────────────────────────────────────
    "matrpt.title":         { en: "📦 MATERIAL PURCHASE REPORT", mr: "📦 साहित्य खरेदी अहवाल" },
    "matrpt.subtitle":      { en: "Filter by date range and generate material purchase summary", mr: "तारीख श्रेणीनुसार फिल्टर करा आणि साहित्य खरेदी सारांश तयार करा" },
    "matrpt.filterTitle":   { en: "REPORT FILTER",          mr: "अहवाल फिल्टर" },
    "matrpt.filterSub":     { en: "Select date range to generate report", mr: "अहवाल तयार करण्यासाठी तारीख श्रेणी निवडा" },
    "matrpt.fromDate":      { en: "From Date *",            mr: "पासून तारीख *" },
    "matrpt.toDate":        { en: "To Date *",              mr: "पर्यंत तारीख *" },
    "matrpt.generate":      { en: "🔍 Generate Report",     mr: "🔍 अहवाल तयार करा" },
    "matrpt.reset":         { en: "↺ Reset",                mr: "↺ रीसेट" },
    "matrpt.records":       { en: "Purchase Records",       mr: "खरेदी नोंदी" },
    "matrpt.selectRange":   { en: "Select a date range and click Generate", mr: "तारीख श्रेणी निवडा आणि तयार करा क्लिक करा" },
    "matrpt.print":         { en: "🖨️ Print",               mr: "🖨️ प्रिंट" },
    "matrpt.totalRecords":  { en: "Total Records",          mr: "एकूण नोंदी" },
    "matrpt.period":        { en: "Period",                 mr: "कालावधी" },
    "matrpt.grossTotal":    { en: "💰 Gross Total",         mr: "💰 एकूण बेरीज" },
    "matrpt.search":        { en: "Search by vendor, material, date…", mr: "विक्रेता, साहित्य, तारीखाने शोधा…" },
    "matrpt.col.no":        { en: "#",                      mr: "#" },
    "matrpt.col.date":      { en: "Purchase Date",          mr: "खरेदी तारीख" },
    "matrpt.col.vendor":    { en: "Vendor",                 mr: "विक्रेता" },
    "matrpt.col.material":  { en: "Material",               mr: "साहित्य" },
    "matrpt.col.qty":       { en: "Qty",                    mr: "प्रमाण" },
    "matrpt.col.rate":      { en: "Rate (₹)",               mr: "दर (₹)" },
    "matrpt.col.total":     { en: "Total Amount (₹)",       mr: "एकूण रक्कम (₹)" },
    "matrpt.grossLbl":      { en: "GROSS TOTAL",            mr: "एकूण बेरीज" },
    "matrpt.noSearch":      { en: "No records match your search.", mr: "आपल्या शोधाशी जुळणाऱ्या नोंदी नाहीत." },
    "matrpt.rows":          { en: "Rows:",                  mr: "ओळी:" },
    "matrpt.showing":       { en: "Showing",                mr: "दाखवत आहे" },
    "matrpt.of":            { en: "of",                     mr: "पैकी" },
    "matrpt.recordsLbl":    { en: "records",                mr: "नोंदी" },
    "matrpt.emptyDefault":  { en: "Select a date range above and click Generate Report to view purchase data.", mr: "वरील तारीख श्रेणी निवडा आणि खरेदी डेटा पाहण्यासाठी अहवाल तयार करा क्लिक करा." },
    "matrpt.emptyNoData":   { en: "No purchase records found for the selected date range.", mr: "निवडलेल्या तारीख श्रेणीसाठी कोणत्याही खरेदी नोंदी आढळल्या नाहीत." },

    // ── CUSTOMER SALES REPORT ────────────────────────────────
    "custrpt.title":        { en: "🧱 CUSTOMER SALES REPORT", mr: "🧱 ग्राहक विक्री अहवाल" },
    "custrpt.subtitle":     { en: "Filter by date range and generate customer sales summary", mr: "तारीख श्रेणीनुसार फिल्टर करा आणि ग्राहक विक्री सारांश तयार करा" },
    "custrpt.records":      { en: "Sales Records",          mr: "विक्री नोंदी" },
    "custrpt.col.date":     { en: "Sales Date",             mr: "विक्री तारीख" },
    "custrpt.col.customer": { en: "Customer",               mr: "ग्राहक" },
    "custrpt.col.brickType":{ en: "Brick Type",             mr: "विटांचा प्रकार" },
    "custrpt.search":       { en: "Search by customer, brick type, date…", mr: "ग्राहक, विटांचा प्रकार, तारीखाने शोधा…" },
    "custrpt.emptyDefault": { en: "Select a date range above and click Generate Report to view sales data.", mr: "वरील तारीख श्रेणी निवडा आणि विक्री डेटा पाहण्यासाठी अहवाल तयार करा क्लिक करा." },
    "custrpt.emptyNoData":  { en: "No sales records found for the selected date range.", mr: "निवडलेल्या तारीख श्रेणीसाठी कोणत्याही विक्री नोंदी आढळल्या नाहीत." },

    // ── VENDOR SPECIFIC PURCHASE REPORT ──────────────────────
    "vsrpt.title":          { en: "📦 VENDOR SPECIFIC PURCHASE REPORT", mr: "📦 विक्रेतानिहाय खरेदी अहवाल" },
    "vsrpt.subtitle":       { en: "Select a vendor and date range to generate their purchase report", mr: "विक्रेता आणि तारीख श्रेणी निवडून त्यांचा खरेदी अहवाल तयार करा" },
    "vsrpt.vendor":         { en: "Vendor",                 mr: "विक्रेता" },
    "vsrpt.selectVendor":   { en: "— Select Vendor —",      mr: "— विक्रेता निवडा —" },
    "vsrpt.selectHint":     { en: "Select a vendor and date range, then click Generate", mr: "विक्रेता आणि तारीख श्रेणी निवडा, मग तयार करा क्लिक करा" },
    "vsrpt.emptyDefault":   { en: "Select a vendor and date range above, then click Generate Report.", mr: "वरील विक्रेता आणि तारीख श्रेणी निवडा, मग अहवाल तयार करा क्लिक करा." },
    "vsrpt.emptyNoData":    { en: "No purchase records found for this vendor in the selected date range.", mr: "निवडलेल्या तारीख श्रेणीत या विक्रेत्याच्या कोणत्याही खरेदी नोंदी आढळल्या नाहीत." },

    // ── CUSTOMER SPECIFIC REPORT ─────────────────────────────
    "csrpt.title":          { en: "👤 CUSTOMER SPECIFIC REPORT", mr: "👤 ग्राहकनिहाय अहवाल" },
    "csrpt.subtitle":       { en: "Select a customer and date range to generate their sales report", mr: "ग्राहक आणि तारीख श्रेणी निवडून त्यांचा विक्री अहवाल तयार करा" },
    "csrpt.customer":       { en: "Customer",                 mr: "ग्राहक" },
    "csrpt.selectCustomer": { en: "— Select Customer —",      mr: "— ग्राहक निवडा —" },
    "csrpt.selectHint":     { en: "Select a customer and date range, then click Generate", mr: "ग्राहक आणि तारीख श्रेणी निवडा, मग तयार करा क्लिक करा" },
    "csrpt.emptyDefault":   { en: "Select a customer and date range above, then click Generate Report.", mr: "वरील ग्राहक आणि तारीख श्रेणी निवडा, मग अहवाल तयार करा क्लिक करा." },
    "csrpt.emptyNoData":    { en: "No sales records found for this customer in the selected date range.", mr: "निवडलेल्या तारीख श्रेणीत या ग्राहकाच्या कोणत्याही विक्री नोंदी आढळल्या नाहीत." },
    "csrpt.totalPaid":      { en: "✅ Total Paid",          mr: "✅ एकूण भरले" },
    "csrpt.remaining":      { en: "⏳ Remaining",           mr: "⏳ उर्वरित" },
    "csrpt.totalPaidLbl":   { en: "✅ TOTAL PAID",          mr: "✅ एकूण भरले" },
    "csrpt.remainingLbl":   { en: "⏳ REMAINING",           mr: "⏳ उर्वरित" },

    // ── SALES REPORT ─────────────────────────────────────────
    "salesrpt.hint":        { en: "● Filter by date range to view sales revenue summary", mr: "● महसूल सारांश पाहण्यासाठी तारीख श्रेणीनुसार फिल्टर करा" },
    "salesrpt.filterTitle": { en: "REPORT FILTERS",         mr: "अहवाल फिल्टर" },
    "salesrpt.filterSub":   { en: "Select date range to view revenue summary", mr: "महसूल सारांश पाहण्यासाठी तारीख श्रेणी निवडा" },
    "salesrpt.fromDate":    { en: "From Date",              mr: "पासून तारीख" },
    "salesrpt.toDate":      { en: "To Date",                mr: "पर्यंत तारीख" },
    "salesrpt.generate":    { en: "🔍 Generate",            mr: "🔍 तयार करा" },
    "salesrpt.kpiCustomers":{ en: "Total Customers",        mr: "एकूण ग्राहक" },
    "salesrpt.kpiPeriod":   { en: "in selected period",     mr: "निवडलेल्या कालावधीत" },
    "salesrpt.kpiRevenue":  { en: "Total Revenue",          mr: "एकूण महसूल" },
    "salesrpt.kpiRevSub":   { en: "based on final amount",  mr: "अंतिम रकमेवर आधारित" },
    "salesrpt.summaryTitle":{ en: "REPORT SUMMARY",         mr: "अहवाल सारांश" },
    "salesrpt.summarySub":  { en: "Detailed breakdown of the generated report", mr: "तयार केलेल्या अहवालाचा तपशीलवार विभाजन" },
    "salesrpt.print":       { en: "🖨 Print Report",         mr: "🖨 अहवाल प्रिंट करा" },
    "salesrpt.fromDateLbl": { en: "From Date",              mr: "पासून तारीख" },
    "salesrpt.toDateLbl":   { en: "To Date",                mr: "पर्यंत तारीख" },
    "salesrpt.totalCust":   { en: "Total Customers",        mr: "एकूण ग्राहक" },
    "salesrpt.totalRev":    { en: "Total Revenue",          mr: "एकूण महसूल" },
    "salesrpt.generated":   { en: "Report Generated",       mr: "अहवाल तयार केला" },
    "salesrpt.empty":       { en: "Select a date range and click Generate to view the sales report.", mr: "विक्री अहवाल पाहण्यासाठी तारीख श्रेणी निवडा आणि तयार करा क्लिक करा." },

    // ── PENDING PAYMENT REPORT ───────────────────────────────
    "pendrpt.title":        { en: "PENDING PAYMENT REPORT", mr: "प्रलंबित देयक अहवाल" },
    "pendrpt.subtitle":     { en: "Track outstanding and partially paid customer balances", mr: "थकीत आणि अंशतः भरलेल्या ग्राहक शिल्लकांचा मागोवा घ्या" },
    "pendrpt.statCustDue":  { en: "Customers Due",          mr: "देय ग्राहक" },
    "pendrpt.statPending":  { en: "Fully Pending",          mr: "पूर्णपणे प्रलंबित" },
    "pendrpt.statPartial":  { en: "Partial",                mr: "अंशतः" },
    "pendrpt.statCollected":{ en: "Total Collected",        mr: "एकूण संकलित" },
    "pendrpt.statOutstanding":{ en: "Total Outstanding",    mr: "एकूण थकीत" },
    "pendrpt.tableTitle":   { en: "Outstanding Payments",   mr: "थकीत देयके" },
    "pendrpt.tableSub":     { en: "Customers with pending or partial balance", mr: "प्रलंबित किंवा अंशतः शिल्लक असलेले ग्राहक" },
    "pendrpt.search":       { en: "Search by customer name…", mr: "ग्राहकाच्या नावाने शोधा…" },
    "pendrpt.col.no":       { en: "#",                      mr: "#" },
    "pendrpt.col.customer": { en: "Customer",               mr: "ग्राहक" },
    "pendrpt.col.totalSales":{ en: "Total Sales",           mr: "एकूण विक्री" },
    "pendrpt.col.paid":     { en: "Paid",                   mr: "भरले" },
    "pendrpt.col.remaining":{ en: "Remaining",              mr: "उर्वरित" },
    "pendrpt.col.progress": { en: "Progress",               mr: "प्रगती" },
    "pendrpt.col.status":   { en: "Status",                 mr: "स्थिती" },
    "pendrpt.noSearch":     { en: "No records match your search.", mr: "आपल्या शोधाशी जुळणाऱ्या नोंदी नाहीत." },
    "pendrpt.empty":        { en: "No outstanding payments — all customers are fully paid!", mr: "कोणतेही थकीत देयके नाहीत — सर्व ग्राहकांनी पूर्ण भरणा केला आहे!" },
    "pendrpt.showing":      { en: "Showing",                mr: "दाखवत आहे" },
    "pendrpt.of":           { en: "of",                     mr: "पैकी" },
    "pendrpt.records":      { en: "records",                mr: "नोंदी" },

    // ── DASHBOARD — SALARY STATUS ────────────────────────────
    "dash.salaryStatus": { en: "Salary Status", mr: "पगार स्थिती" },
    "dash.salaryPaid": { en: "Salary Paid", mr: "पगार दिला" },
    "dash.workers32": { en: "32 workers", mr: "३२ कामगार" },

// ── Labour Ledger Report ────────────────────────────
    "ledger.subtitle": { en: "Advance & salary summary for a selected labour over a date range", mr: "निवडलेल्या लेबरसाठी दिलेल्या कालावधीत अॅडव्हान्स आणि सॅलरी समरी" },
    "ledger.filterTitle": { en: "FILTER", mr: "फिल्टर" },
    "ledger.filterSub": { en: "Select labour & date range", mr: "लेबर आणि डेट रेंज निवडा" },
    "form.labour": { en: "Labour *", mr: "लेबर *" },
    "ledger.fromDate": { en: "From Date *", mr: "फ्रॉम डेट *" },
    "ledger.toDate": { en: "To Date *", mr: "टू डेट *" },
    "ledger.noReport": { en: "No Report Yet", mr: "अजून रिपोर्ट नाही" },
    "ledger.noReportHint": { en: "Select a labour and date range, then click Show Report.", mr: "लेबर आणि डेट रेंज निवडा, मग Show Report वर क्लिक करा" },
    "ledger.showReport": { en: "🔍 Show Report", mr: "🔍 रिपोर्ट दाखवा" },
    "ledger.labourLedger": { en: "Labour Ledger", mr: "कामगार खातेवही" },
    "ledger.noData": { en: "No records found for the selected labour and date range.", mr: "निवडलेल्या कामगार आणि कालावधीसाठी कोणतीही नोंद आढळली नाही." },
    "ledger.advSection": { en: "ADVANCE SECTION", mr: "अॅडव्हान्स विभाग" },
    "ledger.initialAdv": { en: "Initial Advance", mr: "प्रारंभिक अॅडव्हान्स" },
    "ledger.additionalAdv": { en: "Additional Advance", mr: "अतिरिक्त अॅडव्हान्स" },
    "ledger.totalAdv": { en: "Total Advance", mr: "एकूण अॅडव्हान्स" },
    "ledger.deduction": { en: "Deduction", mr: "कपात" },
    "ledger.advBalance": { en: "Advance Balance", mr: "अॅडव्हान्स शिल्लक" },
    "ledger.salSection": { en: "SALARY SECTION", mr: "पगार विभाग" },
    "ledger.salaryEarned": { en: "Salary Earned", mr: "मिळालेला पगार" },
    "ledger.expense": { en: "Expense", mr: "खर्च" },
    "ledger.finalSalary": { en: "Final Salary", mr: "अंतिम पगार" },
    "ledger.paidAmount": { en: "Paid Amount", mr: "भरलेली रक्कम" },
    "ledger.printHeader": { en: "Jyotirling Bricks Suppliers — Labour Ledger Report", mr: "ज्योतिर्लिंग विट सप्लायर्स — कामगार खातेवही अहवाल" },

    // ── COMMON FORM / TABLE LABELS ───────────────────────────
    "common.active":        { en: "Active",                 mr: "सक्रिय" },
    "common.inactive":      { en: "Inactive",               mr: "निष्क्रिय" },
    "common.edit":          { en: "✏ Edit",                 mr: "✏ संपादित करा" },
    "common.delete":        { en: "🗑 Delete",              mr: "🗑 हटवा" },
    "common.cancel":        { en: "Cancel",                 mr: "रद्द करा" },
    "common.yesDelete":     { en: "Yes, Delete",            mr: "होय, हटवा" },
    "common.reset":         { en: "↺ Reset",                mr: "↺ रीसेट" },
    "common.cancelEdit":    { en: "↺ Cancel Edit",          mr: "↺ संपादन रद्द करा" },
    "common.showing":       { en: "Showing",                mr: "दाखवत आहे" },
    "common.of":            { en: "of",                     mr: "पैकी" },
    "common.records":       { en: "records",                mr: "नोंदी" },
    "common.actions":       { en: "Actions",                mr: "क्रिया" },
    "common.search":        { en: "Search",                 mr: "शोधा" },
    "common.total":         { en: "Total",                  mr: "एकूण" },
    "common.paid":          { en: "Paid",                   mr: "भरले" },
    "common.pending":       { en: "Pending",                mr: "प्रलंबित" },
    "common.details":       { en: "Details →",              mr: "तपशील →" },
    "common.all":           { en: "All →",                  mr: "सर्व →" },

    // ── SHARED FORM FIELD LABELS ─────────────────────────────
    "form.status":          { en: "Status",                 mr: "स्थिती" },
    "form.address":         { en: "Address",                mr: "पत्ता" },
    "form.mobileNumber":    { en: "Mobile Number *",        mr: "मोबाइल नंबर *" },
    "form.mobile":          { en: "Mobile",                 mr: "मोबाइल" },
    "form.date":            { en: "Date",                   mr: "तारीख" },
    "form.amount":          { en: "Amount (₹) *",           mr: "रक्कम (₹) *" },
    "form.reason":          { en: "Reason *",               mr: "कारण *" },
    "form.labour":          { en: "Labour *",               mr: "कामगार *" },
    "form.quantity":        { en: "Quantity *",             mr: "प्रमाण *" },
    "form.rate":            { en: "Rate (₹) *",             mr: "दर (₹) *" },
    "form.searchNameMobile":{ en: "Search by name or mobile…", mr: "नाव किंवा मोबाइलने शोधा…" },

    // ── VENDOR MASTER ────────────────────────────────────────
    "vendor.addVendor":     { en: "Add Vendor",             mr: "विक्रेता जोडा" },
    "vendor.editVendor":    { en: "Edit Vendor",            mr: "विक्रेता संपादित करा" },
    "vendor.updateVendor":  { en: "💾 Update Vendor",       mr: "💾 विक्रेता अपडेट करा" },
    "vendor.registerVendor":{ en: "＋ Register Vendor",     mr: "＋ विक्रेता नोंदवा" },
    "vendor.vendorName":    { en: "Vendor Name *",          mr: "विक्रेत्याचे नाव *" },
    "vendor.gstNumber":     { en: "GST Number",             mr: "जीएसटी क्रमांक" },
    "vendor.vendorList":    { en: "Vendor List",            mr: "विक्रेता यादी" },
    "vendor.vendorCol":     { en: "Vendor",                 mr: "विक्रेता" },
    "vendor.deleteTitle":   { en: "⚠️ Delete Vendor?",      mr: "⚠️ विक्रेता हटवायचा?" },

    // ── MATERIAL MASTER ──────────────────────────────────────
    "material.addMaterial":   { en: "Add Material",         mr: "साहित्य जोडा" },
    "material.editMaterial":  { en: "Edit Material",        mr: "साहित्य संपादित करा" },
    "material.updateMaterial":{ en: "💾 Update Material",   mr: "💾 साहित्य अपडेट करा" },
    "material.addBtn":        { en: "＋ Add Material",       mr: "＋ साहित्य जोडा" },
    "material.materialName":  { en: "Material Name *",      mr: "साहित्याचे नाव *" },
    "material.unit":          { en: "Unit *",               mr: "एकक *" },
    "material.materialList":  { en: "Material List",        mr: "साहित्य यादी" },
    "material.materialCol":   { en: "Material Name",        mr: "साहित्याचे नाव" },
    "material.unitCol":       { en: "Unit",                 mr: "एकक" },
    "material.deleteTitle":   { en: "⚠️ Delete Material?",  mr: "⚠️ साहित्य हटवायचे?" },

    // ── MATERIAL PURCHASE ────────────────────────────────────
    "purchase.newPurchase":   { en: "NEW PURCHASE",         mr: "नवीन खरेदी" },
    "purchase.editPurchase":  { en: "EDIT PURCHASE",        mr: "खरेदी संपादित करा" },
    "purchase.purchaseDate":  { en: "Purchase Date *",      mr: "खरेदी तारीख *" },
    "purchase.vendor":        { en: "Vendor *",             mr: "विक्रेता *" },
    "purchase.material":      { en: "Material *",           mr: "साहित्य *" },
    "purchase.pricing":       { en: "📋 Purchase Details",  mr: "📋 खरेदी तपशील" },
    "purchase.payment":       { en: "💳 Payment",           mr: "💳 देयक" },
    "purchase.paidAmount":    { en: "Paid Amount (₹)",      mr: "भरलेली रक्कम (₹)" },
    "purchase.pendingAmount": { en: "Pending Amount (Auto)",mr: "प्रलंबित रक्कम (स्वयं)" },
    "purchase.totalMinusPaid": {
        en: "Total − Paid",
        mr: "🧮 टोटल मायनस पेड"
    },
    "purchase.totalAmount": { en: "Total Amount (Auto)", mr: "एकूण रक्कम (स्वयं)" },
    "purchase.savePurchase":  { en: "＋ Save Purchase",      mr: "＋ खरेदी जतन करा" },
    "purchase.updatePurchase":{ en: "💾 Update Purchase",   mr: "💾 खरेदी अपडेट करा" },
    "purchase.purchaseList":  { en: "Purchase List",        mr: "खरेदी यादी" },
    "purchase.deleteTitle": { en: "⚠️ Delete Purchase?", mr: "⚠️ खरेदी हटवायची?" },
    "purchase.deleteTitle": { en: "⚠️ Delete Purchase?", mr: "⚠️ खरेदी हटवायची?" },
    "purchase.subtitle": { en: "Record a material purchase", mr: "मटेरियल पर्चेस रेकॉर्ड करा" },
    "purchase.listSub": { en: "All recorded material purchases", mr: "सर्व रेकॉर्ड केलेले मटेरियल पर्चेस" },
    "purchase.newSub": { en: "Record a material purchase", mr: "मटेरियल पर्चेस रेकॉर्ड करा" },
    "purchase.search": { en: "Search by vendor, material…", mr: "व्हेंडर, मटेरियल ने सर्च करा…" },

    // ── LABOUR MASTER ────────────────────────────────────────
    "labour.addLabour":       { en: "Add Labour",           mr: "कामगार जोडा" },
    "labour.editLabour":      { en: "Edit Labour",          mr: "कामगार संपादित करा" },
    "labour.addBtn":          { en: "＋ Add Labour",         mr: "＋ कामगार जोडा" },
    "labour.updateBtn":       { en: "💾 Update Labour",      mr: "💾 कामगार अपडेट करा" },
    "labour.labourName":      { en: "Labour Name *",        mr: "कामगाराचे नाव *" },
    "labour.dailyWage":       { en: "Daily Wage (₹)",       mr: "दैनिक मजुरी (₹)" },
    "labour.joiningDate":     { en: "Joining Date",         mr: "रुजू तारीख" },
    "labour.labourList":      { en: "Labour List",          mr: "कामगार यादी" },
    "labour.dailyWageCol":    { en: "Daily Wage",           mr: "दैनिक मजुरी" },
    "labour.joiningDateCol":  { en: "Joining Date",         mr: "रुजू तारीख" },
    "labour.deleteTitle":     { en: "⚠️ Delete Labour?",    mr: "⚠️ कामगार हटवायचा?" },

    // ── LABOUR ADVANCE ───────────────────────────────────────
    "advance.recordAdvance":  { en: "Record Advance",       mr: "आगाऊ नोंदवा" },
    "advance.editAdvance":    { en: "Edit Advance",         mr: "आगाऊ संपादित करा" },
    "advance.saveBtn":        { en: "＋ Record Advance",     mr: "＋ आगाऊ नोंदवा" },
    "advance.updateBtn":      { en: "💾 Update Advance",    mr: "💾 आगाऊ अपडेट करा" },
    "advance.advanceDate":    { en: "Advance Date *",       mr: "आगाऊ तारीख *" },
    "advance.advanceType":    { en: "Advance Type *",       mr: "आगाऊ प्रकार *" },
    "advance.advanceAmount":  { en: "Advance Amount (₹) *", mr: "आगाऊ रक्कम (₹) *" },
    "advance.advanceRecords": { en: "Advance Records",      mr: "आगाऊ नोंदी" },
    "advance.advanceType.col":{ en: "Advance Type",         mr: "आगाऊ प्रकार" },
    "advance.labourTotal":    { en: "Labour Total",         mr: "कामगार एकूण" },
    "advance.deleteTitle":    { en: "⚠️ Delete Advance?",   mr: "⚠️ आगाऊ हटवायचे?" },
    "advance.records": { en: "Records", mr: "रेकॉर्ड्स" },
    "advance.totalDisbursed": { en: "Total Disbursed", mr: "टोटल डिस्बर्स्ड" },
    "advance.totalRecords": { en: "Total Records", mr: "टोटल रेकॉर्ड्स" },
    "advance.search": { en: "Search by labour or type…", mr: "लेबर किंवा टाइप ने सर्च करा…" },
   

    // ── LABOUR WORK ENTRY ────────────────────────────────────
    "work.workEntry":         { en: "WORK ENTRY",           mr: "काम नोंद" },
    "work.editEntry":         { en: "EDIT ENTRY",           mr: "नोंद संपादित करा" },
    "work.saveBtn":           { en: "＋ Save Entry",         mr: "＋ नोंद जतन करा" },
    "work.updateBtn":         { en: "💾 Update Entry",       mr: "💾 नोंद अपडेट करा" },
    "work.workDate":          { en: "Work Date *",          mr: "काम तारीख *" },
    "work.dailyWage":         { en: "Daily Wage (₹) *",     mr: "दैनिक मजुरी (₹) *" },
    "work.daysWorked":        { en: "Days Worked *",        mr: "काम केलेले दिवस *" },
    "work.totalSalary":       { en: "Total Salary (Auto)",  mr: "एकूण पगार (स्वयं)" },
    "work.workRecords":       { en: "WORK RECORDS",         mr: "काम नोंदी" },
    "work.daysCol":           { en: "Days",                 mr: "दिवस" },
    "work.totalSalaryCol":    { en: "Total Salary",         mr: "एकूण पगार" },
    "work.deleteTitle":       { en: "⚠️ Delete Entry?",     mr: "⚠️ नोंद हटवायची?" },

    // ── LABOUR EXPENSE ───────────────────────────────────────
    "expense.addExpense":     { en: "Add Expense",          mr: "खर्च जोडा" },
    "expense.editExpense":    { en: "Edit Expense",         mr: "खर्च संपादित करा" },
    "expense.saveBtn":        { en: "＋ Save Expense",       mr: "＋ खर्च जतन करा" },
    "expense.updateBtn":      { en: "💾 Update Expense",     mr: "💾 खर्च अपडेट करा" },
    "expense.expenseDate":    { en: "Expense Date *",       mr: "खर्च तारीख *" },
    "expense.expenseRecords": { en: "Expense Records",      mr: "खर्च नोंदी" },
    "expense.reasonCol":      { en: "Reason",               mr: "कारण" },
    "expense.deleteTitle":    { en: "⚠️ Delete Expense?",   mr: "⚠️ खर्च हटवायचा?" },

    // ── ADVANCE DEDUCTION ────────────────────────────────────
    "deduction.newDeduction": { en: "NEW DEDUCTION",        mr: "नवीन कपात" },
    "deduction.editDeduction":{ en: "EDIT DEDUCTION",       mr: "कपात संपादित करा" },
    "deduction.saveBtn":      { en: "＋ Save Deduction",     mr: "＋ कपात जतन करा" },
    "deduction.updateBtn":    { en: "💾 Update Deduction",   mr: "💾 कपात अपडेट करा" },
    "deduction.deductionDate":{ en: "Deduction Date *",     mr: "कपात तारीख *" },
    "deduction.deductionAmt": { en: "Deduction Amount (₹) *",mr: "कपात रक्कम (₹) *" },
    "deduction.deductionList":{ en: "Deduction List",       mr: "कपात यादी" },
    "deduction.deleteTitle":  { en: "⚠️ Delete Deduction?", mr: "⚠️ कपात हटवायची?" },
    "deduction.title": { en: "ADVANCE DEDUCTION", mr: "अॅडव्हान्स डिडक्शन" },
    "deduction.subtitle": { en: "Deduct from labour advance balance", mr: "लेबर अॅडव्हान्स बॅलन्स मधून डिडक्ट करा" },
    "deduction.newSub": { en: "Record an advance deduction", mr: "अॅडव्हान्स डिडक्शन रेकॉर्ड करा" },
    "deduction.listSub": { en: "All recorded advance deductions", mr: "सर्व रेकॉर्ड केलेले अॅडव्हान्स डिडक्शन्स" },
    "deduction.totalDeducted": { en: "Total Deducted", mr: "टोटल डिडक्टेड" },
    "deduction.records": { en: "Records", mr: "रेकॉर्ड्स" },
    "deduction.search": { en: "Search by labour, reason…", mr: "लेबर, रिझन ने सर्च करा…" },

    // ── SALARY PAYMENT ───────────────────────────────────────
    "salary.newSalary":       { en: "NEW SALARY",           mr: "नवीन पगार" },
    "salary.search": { en: "Search by labour name or status…", mr: "लेबर नाव किंवा स्टेटस ने सर्च करा…" },
    "salary.editSalary":      { en: "EDIT SALARY",          mr: "पगार संपादित करा" },
    "salary.saveBtn":         { en: "＋ Save Salary",        mr: "＋ पगार जतन करा" },
    "salary.updateBtn":       { en: "💾 Update Salary",      mr: "💾 पगार अपडेट करा" },
    "salary.salaryDate":      { en: "Salary Date *",        mr: "पगार तारीख *" },
    "salary.totalSalary":     { en: "Total Salary (from Work Log)", mr: "एकूण पगार (कामाच्या नोंदीतून)" },
    "salary.totalExpense":    { en: "Total Expense (Deductions)", mr: "एकूण खर्च (कपात)" },
    "salary.finalSalary":     { en: "Final Salary (Auto)",  mr: "अंतिम पगार (स्वयं)" },
    "salary.paidAmount":      { en: "Paid Amount (₹) *",    mr: "भरलेली रक्कम (₹) *" },
    "salary.paymentStatus":   { en: "Payment Status *",     mr: "देयक स्थिती *" },
    "salary.salaryList":      { en: "Salary Payment List",  mr: "पगार देयक यादी" },
    "salary.expenseCol":      { en: "Expense",              mr: "खर्च" },
    "salary.finalCol":        { en: "Final Salary",         mr: "अंतिम पगार" },
    "salary.deleteTitle": { en: "⚠️ Delete Salary Record?", mr: "⚠️ पगार नोंद हटवायची?" },
    "salary.title": { en: "SALARY PAYMENT", mr: "सॅलरी पेमेंट" },
    "salary.subtitle": { en: "Record and manage labour salary payments", mr: "लेबर सॅलरी पेमेंट्स रेकॉर्ड आणि मॅनेज करा" },
    "salary.newSub": { en: "Record a salary payment", mr: "सॅलरी पेमेंट रेकॉर्ड करा" },
    "salary.listSub": { en: "All recorded salary payments", mr: "सर्व रेकॉर्ड केलेले सॅलरी पेमेंट्स" },
    "salary.chipSalary": { en: "Salary", mr: "सॅलरी" },
    "salary.chipExpense": { en: "Expense", mr: "एक्स्पेन्स" },
    "salary.chipFinal": { en: "Final", mr: "फायनल" },
    "salary.labourDetails": { en: "👷 Labour Details", mr: "👷 लेबर डिटेल्स" },
    "salary.labour": { en: "Labour *", mr: "लेबर *" },
    "salary.salaryDateLbl": { en: "Salary Date *", mr: "सॅलरी डेट *" },
    "salary.dailyWageLbl": { en: "Daily Wage (₹) *", mr: "डेली वेज (₹) *" },
    "salary.workingDays": { en: "Working Days *", mr: "वर्किंग डेज *" },
    "salary.salaryCalc": { en: "💰 Salary Calculation", mr: "💰 सॅलरी कॅल्क्युलेशन" },
    "salary.totalSalaryAuto": { en: "Total Salary (Auto)", mr: "टोटल सॅलरी (ऑटो)" },
    "salary.calcHint1": { en: "🧮 Daily Wage × Working Days =", mr: "🧮 डेली वेज × वर्किंग डेज =" },
    "salary.deductFinal": { en: "🧾 Deductions & Final", mr: "🧾 डिडक्शन्स & फायनल" },
    "salary.totalExpenseLbl": { en: "Total Expense / Deductions (₹) *", mr: "टोटल एक्स्पेन्स / डिडक्शन्स (₹) *" },
    "salary.finalSalaryAuto": { en: "Final Salary (Auto)", mr: "फायनल सॅलरी (ऑटो)" },
    "salary.calcHint2": { en: "🧮 Total Salary − Expenses =", mr: "🧮 टोटल सॅलरी − एक्स्पेन्सेस =" },
    "salary.paymentLbl": { en: "💳 Payment", mr: "💳 पेमेंट" },
    "salary.paidAmountLbl": { en: "Paid Amount (₹) *", mr: "पेड अमाउंट (₹) *" },
    "salary.paymentStatusLbl": { en: "Payment Status *", mr: "पेमेंट स्टेटस *" },
    "salary.deleteTitle": { en: "⚠️ Delete Salary Record?", mr: "⚠️ पगार नोंद हटवायची?" },
    "salary.dailyWageCol": { en: "Daily Wage", mr: "दैनंदिन मजुरी" },
    "salary.daysCol": { en: "Days", mr: "दिवस" },
    "salary.totalSalaryCol": { en: "Total Salary", mr: "एकूण पगार" },
    "salary.deleteTitle": { en: "⚠️ Delete Salary Record?", mr: "⚠️ पगार नोंद हटवायची?" },
    "salary.deleteTitle": { en: "⚠️ Delete Salary Record?", mr: "⚠️ पगार नोंद हटवायची?" },
    "salary.deleteTitle": { en: "⚠️ Delete Salary Record?", mr: "⚠️ पगार नोंद हटवायची?" },

    // ── BRICK PRODUCTION ─────────────────────────────────────
    "prod.newEntry":          { en: "NEW ENTRY",            mr: "नवीन नोंद" },
    "prod.editEntry":         { en: "EDIT ENTRY",           mr: "नोंद संपादित करा" },
    "prod.saveBtn":           { en: "＋ Save Entry",         mr: "＋ नोंद जतन करा" },
    "prod.updateBtn":         { en: "💾 Update Entry",       mr: "💾 नोंद अपडेट करा" },
    "prod.productionDate":    { en: "Production Date *",    mr: "उत्पादन तारीख *" },
    "prod.brickType":         { en: "Brick Type *",         mr: "विटांचा प्रकार *" },
    "prod.productionRecords": { en: "Production Records",   mr: "उत्पादन नोंदी" },
    "prod.brickTypeCol":      { en: "Brick Type",           mr: "विटांचा प्रकार" },
    "prod.quantityCol":       { en: "Quantity",             mr: "प्रमाण" },
    "prod.deleteTitle":       { en: "⚠️ Delete Production Entry?", mr: "⚠️ उत्पादन नोंद हटवायची?" },
    "prod.title": { en: "BRICK PRODUCTION", mr: "ब्रिक प्रॉडक्शन" },
    "prod.subtitle": { en: "Record daily brick production entries", mr: "डेली ब्रिक प्रॉडक्शन एंट्रीज रेकॉर्ड करा" },
    "prod.newSub": { en: "Record today's production", mr: "आजचे प्रॉडक्शन रेकॉर्ड करा" },
    "prod.detailsLbl": { en: "🧱 Production Details", mr: "🧱 प्रॉडक्शन डिटेल्स" },
    "prod.deleteTitle":       { en: "⚠️ Delete Production Entry?", mr: "⚠️ उत्पादन नोंद हटवायची?" },
    "prod.listSub": { en: "All daily production entries", mr: "सर्व डेली प्रॉडक्शन एंट्रीज" },
    "prod.totalRecords": { en: "Total Records", mr: "टोटल रेकॉर्ड्स" },
    "prod.totalBricks": { en: "Total Bricks", mr: "टोटल ब्रिक्स" },
    "prod.listSub": { en: "All daily production entries", mr: "सर्व डेली प्रॉडक्शन एंट्रीज" },
    "prod.listSub": { en: "All daily production entries", mr: "सर्व डेली प्रॉडक्शन एंट्रीज" },
    "prod.search": { en: "Search by brick type or date…", mr: "ब्रिक टाइप किंवा डेट ने सर्च करा…" },

    // ── CUSTOMER MASTER ──────────────────────────────────────
    "customer.addCustomer":   { en: "ADD CUSTOMER",         mr: "ग्राहक जोडा" },
    "customer.editCustomer":  { en: "EDIT CUSTOMER",        mr: "ग्राहक संपादित करा" },
    "customer.saveBtn":       { en: "＋ Add Customer",       mr: "＋ ग्राहक जोडा" },
    "customer.updateBtn":     { en: "💾 Update Customer",    mr: "💾 ग्राहक अपडेट करा" },
    "customer.customerName":  { en: "Customer Name *",      mr: "ग्राहकाचे नाव *" },
    "customer.customerList":  { en: "Customer List",        mr: "ग्राहक यादी" },
    "customer.customerCol":   { en: "Customer",             mr: "ग्राहक" },
    "customer.deleteTitle":   { en: "⚠️ Delete Customer?",  mr: "⚠️ ग्राहक हटवायचा?" },
    "customer.title": { en: "CUSTOMER MASTER", mr: "कस्टमर मास्टर" },
    "customer.subtitle": { en: "Manage all registered customers", mr: "सर्व रजिस्टर केलेले कस्टमर्स मॅनेज करा" },
    "customer.deleteTitle":   { en: "⚠️ Delete Customer?",  mr: "⚠️ ग्राहक हटवायचा?" },
    "customer.newSub": { en: "Register a new customer", mr: "नवीन कस्टमर रजिस्टर करा" },
    "customer.detailsLbl": { en: "👤 Customer Details", mr: "👤 कस्टमर डिटेल्स" },
    "customer.listSub": { en: "All Registered Customers", mr: "सर्व रजिस्टर केलेले कस्टमर्स" },
    "customer.totalCustomers": { en: "Total Customers", mr: "टोटल कस्टमर्स" },
    "customer.deleteTitle":   { en: "⚠️ Delete Customer?",  mr: "⚠️ ग्राहक हटवायचा?" },
    "customer.search": { en: "Search by customer name…", mr: "कस्टमर नावाने सर्च करा…" },
   

    // ── BRICK SALES ──────────────────────────────────────────
    "sales.newSale":          { en: "NEW SALE",             mr: "नवीन विक्री" },
    "sales.editSale":         { en: "EDIT SALE",            mr: "विक्री संपादित करा" },
    "sales.saveBtn":          { en: "＋ Save Sale",          mr: "＋ विक्री जतन करा" },
    "sales.updateBtn":        { en: "💾 Update Sale",        mr: "💾 विक्री अपडेट करा" },
    "sales.salesDate":        { en: "Sales Date *",         mr: "विक्री तारीख *" },
    "sales.customer":         { en: "Customer *",           mr: "ग्राहक *" },
    "sales.brickType":        { en: "Brick Type *",         mr: "विटांचा प्रकार *" },
    "sales.totalAmount":      { en: "Total Amount (Auto)",  mr: "एकूण रक्कम (स्वयं)" },
    "sales.paidAmount":       { en: "Paid Amount (₹)",      mr: "भरलेली रक्कम (₹)" },
    "sales.pendingAmount":    { en: "Pending Amount (Auto)",mr: "प्रलंबित रक्कम (स्वयं)" },
    "sales.salesList":        { en: "Sales List",           mr: "विक्री यादी" },
    "sales.deleteTitle":      { en: "⚠️ Delete Sale?",      mr: "⚠️ विक्री हटवायची?" },
    "sales.title": { en: "BRICK SALES", mr: "ब्रिक सेल्स" },
    "sales.subtitle": { en: "Record and manage brick sales transactions", mr: "ब्रिक सेल्स ट्रान्झॅक्शन्स रेकॉर्ड आणि मॅनेज करा" },
    "sales.newSub": { en: "Record a brick sale", mr: "ब्रिक सेल रेकॉर्ड करा" },
    "sales.detailsLbl": { en: "📋 Sale Details", mr: "📋 सेल डिटेल्स" },
    "sales.pricingLbl": { en: "💰 Pricing", mr: "💰 प्रायसिंग" },
    "sales.calcHint1": { en: "🧮 Qty × Rate =", mr: "🧮 क्वांटिटी × रेट =" },
    "sales.paymentLbl": { en: "💳 Payment", mr: "💳 पेमेंट" },
    "sales.chipTotal": { en: "Total", mr: "टोटल" },
    "sales.chipPaid": { en: "Paid", mr: "पेड" },
    "sales.chipPending": { en: "Pending", mr: "पेंडिंग" },
    "sales.listSub": { en: "All recorded brick sales", mr: "सर्व रेकॉर्ड केलेले ब्रिक सेल्स" },
    "sales.calcHint2": { en: "🧮 Total − Paid =", mr: "🧮 टोटल − पेड =" },
    "sales.search": { en: "Search by customer, brick type…", mr: "कस्टमर, ब्रिक टाइप ने सर्च करा…" },

    // ── CUSTOMER PAYMENT ─────────────────────────────────────
    "payment.newPayment":     { en: "NEW PAYMENT",          mr: "नवीन देयक" },
    "payment.editPayment":    { en: "EDIT PAYMENT",         mr: "देयक संपादित करा" },
    "payment.saveBtn":        { en: "+ Save Payment",       mr: "+ देयक जतन करा" },
    "payment.updateBtn":      { en: "💾 Update Payment",     mr: "💾 देयक अपडेट करा" },
    "payment.paymentDate":    { en: "Payment Date *",       mr: "देयक तारीख *" },
    "payment.customer":       { en: "Customer *",           mr: "ग्राहक *" },
    "payment.paymentMode":    { en: "Payment Mode *",       mr: "देयक पद्धत *" },
    "payment.paymentStatus":  { en: "Payment Status *",     mr: "देयक स्थिती *" },
    "payment.paymentList":    { en: "Payment List",         mr: "देयक यादी" },
    "payment.modeCol":        { en: "Mode",                 mr: "पद्धत" },
    "payment.deleteTitle": { en: "⚠️ Delete Payment?", mr: "⚠️ देयक हटवायचे?" },
    "payment.title": { en: "CUSTOMER PAYMENTS", mr: "कस्टमर पेमेंट्स" },
    "payment.subtitle": { en: "Record and manage customer payment transactions", mr: "कस्टमर पेमेंट ट्रान्झॅक्शन्स रेकॉर्ड आणि मॅनेज करा" },
    "payment.newSub": { en: "Record a customer payment", mr: "कस्टमर पेमेंट रेकॉर्ड करा" },
    "payment.amountLbl": { en: "💰 AMOUNT", mr: "💰 अमाउंट" },
    "payment.payInfoLbl": { en: "🏦 PAYMENT INFO", mr: "🏦 पेमेंट इन्फो" },
    "payment.listSub": { en: "All recorded customer payments", mr: "सर्व रेकॉर्ड केलेले कस्टमर पेमेंट्स" },
    "payment.totalCollected": { en: "Total Collected", mr: "टोटल कलेक्टेड" },
    "payment.records": { en: "Records", mr: "रेकॉर्ड्स" },
    "payment.search": { en: "Search by customer, mode, status...", mr: "कस्टमर, मोड, स्टेटस ने सर्च करा…" },
    "payment.totalCollected": { en: "Total Collected", mr: "टोटल कलेक्टेड" },
    "payment.totalCollected": { en: "Total Collected", mr: "टोटल कलेक्टेड" },
    "payment.totalCollected": { en: "Total Collected", mr: "टोटल कलेक्टेड" },
    "payment.totalCollected": { en: "Total Collected", mr: "टोटल कलेक्टेड" },
    "payment.totalCollected": { en: "Total Collected", mr: "टोटल कलेक्टेड" },


    // ── Vendor PAYMENT ─────────────────────────────────────
    "vendorpay.subtitle": { en: "Record and manage vendor payment transactions", mr: "व्हेंडर पेमेंट ट्रान्झॅक्शन्स रेकॉर्ड आणि मॅनेज करा" },
    "vendorpay.newPayment": { en: "NEW PAYMENT", mr: "न्यू पेमेंट" },
    "vendorpay.newSub": { en: "Record a vendor payment", mr: "न्यू पेमेंट" },
    "vendorpay.detailsLbl": { en: "PAYMENT DETAILS", mr: "पेमेंट डिटेल्स" },
    "vendorpay.paymentDate": { en: "Payment Date *", mr: "पेमेंट डेट *" },
    "vendorpay.vendor": { en: "Vendor *", mr: "व्हेंडर *" },
    "vendorpay.amountLbl": { en: "💰 AMOUNT *", mr: "💰 अमाउंट *" },
    "vendorpay.amount": { en: "Amount (₹) *", mr: "अमाउंट (₹) *" },
    "vendorpay.paymentMode": { en: "Payment Mode *", mr: "पेमेंट मोड *" },
    "vendorpay.payInfoLbl": { en: "🏦 PAYMENT INFO", mr: "🏦 पेमेंट इन्फो" },
    "vendorpay.paymentStatus": { en: "Payment Status *", mr: "पेमेंट स्टेटस *" },
    "vendorpay.listTitle": { en: "Payment List", mr: "पेमेंट लिस्ट" },
    "vendorpay.listSub": { en: "All recorded vendor payments", mr: "सर्व रेकॉर्ड केलेले व्हेंडर पेमेंट्स" },
    "vendorpay.totalPaid": { en: "Total Paid", mr: "टोटल पेड" },
    "vendorpay.records": { en: "Records", mr: "रेकॉर्ड्स" },
    "vendorpay.col.date": { en: "Date", mr: "डेट" },
    "vendorpay.col.vendor": { en: "Vendor", mr: "व्हेंडर" },
    "vendorpay.col.amount": { en: "Amount", mr: "अमाउंट" },
    "vendorpay.col.mode": { en: "Mode", mr: "मोड" },
    "vendorpay.col.status": { en: "Status", mr: "स्टेटस" },
    "vendorpay.col.actions": { en: "Actions", mr: "अॅक्शन्स" },
    "vendorpay.rows": { en: "Rows", mr: "रोज़" },
    "vendorpay.search": { en: "Search by vendor, mode, status...", mr:"व्हेंडर, मोड, स्टेटस ने सर्च करा..."},

    // ── BRICK TYPE MASTER ─────────────────────────────────────
    "nav.brickTypeMaster":      { en: "🧱 Brick Type Master",      mr: "🧱 विटांचा प्रकार मास्टर" },
    "brickType.addBrickType":   { en: "Add Brick Type",            mr: "विटांचा प्रकार जोडा" },
    "brickType.editBrickType":  { en: "Edit Brick Type",           mr: "विटांचा प्रकार संपादित करा" },
    "brickType.addBtn":         { en: "＋ Add Brick Type",          mr: "＋ विटांचा प्रकार जोडा" },
    "brickType.updateBrickType":{ en: "💾 Update Brick Type",      mr: "💾 विटांचा प्रकार अपडेट करा" },
    "brickType.brickTypeName":  { en: "Brick Type Name *",         mr: "विटांच्या प्रकाराचे नाव *" },
    "brickType.brickTypeList":  { en: "Brick Type List",           mr: "विटांच्या प्रकाराची यादी" },
    "brickType.brickTypeCol":   { en: "Brick Type Name",           mr: "विटांच्या प्रकाराचे नाव" },
    "brickType.deleteTitle":    { en: "⚠️ Delete Brick Type?",     mr: "⚠️ विटांचा प्रकार हटवायचा?" },
    "brickType.noSearch":       { en: "No brick types match your search.", mr: "आपल्या शोधाशी जुळणारे विटांचे प्रकार नाहीत." },
    "brickType.brickTypes":     { en: "brick types",               mr: "विटांचे प्रकार" },

    // ── VENDOR PENDING PAYMENT REPORT ───────────────────────
    "vpendrpt.title":           { en: "VENDOR PENDING PAYMENT REPORT",                  mr: "विक्रेता प्रलंबित देयक अहवाल" },
    "vpendrpt.subtitle":        { en: "Track outstanding and partially paid vendor balances", mr: "थकीत आणि अंशतः भरलेल्या विक्रेता शिल्लकांचा मागोवा घ्या" },
    "vpendrpt.statVendorsDue":  { en: "Vendors Due",                                    mr: "देय विक्रेते" },
    "vpendrpt.statPending":     { en: "Fully Pending",                                  mr: "पूर्णपणे प्रलंबित" },
    "vpendrpt.statPartial":     { en: "Partial",                                        mr: "अंशतः" },
    "vpendrpt.statTotalPaid":   { en: "Total Paid",                                     mr: "एकूण भरले" },
    "vpendrpt.statOutstanding": { en: "Total Outstanding",                              mr: "एकूण थकीत" },
    "vpendrpt.tableTitle":      { en: "Vendor Outstanding Payments",                    mr: "विक्रेता थकीत देयके" },
    "vpendrpt.tableSub":        { en: "Vendors with pending or partial balance",         mr: "प्रलंबित किंवा अंशतः शिल्लक असलेले विक्रेते" },
    "vpendrpt.search":          { en: "Search by vendor name…",                         mr: "विक्रेत्याच्या नावाने शोधा…" },
    "vpendrpt.col.vendor":      { en: "Vendor",                                         mr: "विक्रेता" },
    "vpendrpt.col.totalPurchase":{ en: "Total Purchase",                                mr: "एकूण खरेदी" },
    "vpendrpt.col.paid":        { en: "Paid",                                           mr: "भरले" },
    "vpendrpt.col.remaining":   { en: "Remaining",                                      mr: "उर्वरित" },
    "vpendrpt.col.progress":    { en: "Progress",                                       mr: "प्रगती" },
    "vpendrpt.col.status":      { en: "Status",                                         mr: "स्थिती" },
};



// ── LANGUAGE ENGINE ─────────────────────────────────────────

/**
 * applyLanguage(lang)
 * Loops through every element with [data-i18n] and replaces
 * its textContent with the translation for the given language.
 * @param {string} lang  - "en" or "mr"
 */
function applyLanguage(lang) {
    // Find every element that has a data-i18n attribute
    document.querySelectorAll('[data-i18n]').forEach(el => {
        const key = el.getAttribute('data-i18n');

        // Look up the key in our dictionary
        if (TRANSLATIONS[key] && TRANSLATIONS[key][lang]) {
            el.textContent = TRANSLATIONS[key][lang];
        }
    });

    // Also update placeholder attributes (search boxes etc.)
    document.querySelectorAll('[data-i18n-placeholder]').forEach(el => {
        const key = el.getAttribute('data-i18n-placeholder');
        if (TRANSLATIONS[key] && TRANSLATIONS[key][lang]) {
            el.placeholder = TRANSLATIONS[key][lang];
        }
    });

    // Save the chosen language to localStorage so it persists across pages
    localStorage.setItem('brickErpLang', lang);

    // Update the dropdown to show the current selection
    const dropdown = document.getElementById('langDropdown');
    if (dropdown) dropdown.value = lang;
}

/**
 * changeLanguage(lang)
 * Called directly by the dropdown's onchange event.
 * @param {string} lang  - "en" or "mr"
 */
function changeLanguage(lang) {
    applyLanguage(lang);
}

/**
 * On every page load, read the saved language from localStorage
 * and apply it automatically — so the user's choice is remembered.
 */
document.addEventListener('DOMContentLoaded', function () {
    // Version bump — clears stale localStorage cache when translations change
    const I18N_VERSION = '7';
    if (localStorage.getItem('brickErpLangVer') !== I18N_VERSION) {
        localStorage.removeItem('brickErpLang');
        localStorage.setItem('brickErpLangVer', I18N_VERSION);
    }
    // Read saved language; default to English if nothing saved yet
    const savedLang = localStorage.getItem('brickErpLang') || 'en';
    applyLanguage(savedLang);
});
