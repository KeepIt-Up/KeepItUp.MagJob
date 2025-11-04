import { Component, inject, ViewChild, ElementRef, AfterViewInit } from '@angular/core';
import { NavbarComponent } from '../../shared/components/navbar/navbar.component';
import { ButtonComponent } from '../../shared/components/button/button.component';
import { FooterComponent } from '../../shared/components/footer/footer.component';
import { NgIcon } from '@ng-icons/core';
import { AuthService } from '@core/services/auth.service';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Shift } from '../shift/models/shift.model';
import { ShiftApiService } from '../shift/services/shift.api';
import Chart from 'chart.js/auto';
import { UserService } from '../users/services/user.service';
import { Organization } from '@organizations/models/organization.model';
import { OrganizationApiService } from '@organizations/services/organization.api.service';


interface WorkEntry {
    date: string;
    startTime: string;
    endTime: string;
  }
  
  interface WeekGroup {
    weekNumber: number;
    startDate: Date;
    endDate: Date;
    entries: WorkEntry[];
    isExpanded: boolean;
  }
  
  interface Employee {
    id: string;
    name: string;
    workEntries: WorkEntry[];
    showDetails: boolean;
    weekGroups?: WeekGroup[];
  }



@Component({
  selector: 'app-workevidence',
  standalone: true,
  imports: [NavbarComponent, ButtonComponent, FooterComponent, CommonModule, FormsModule],
  templateUrl: './workevidence.component.html',
  styleUrl: './workevidence.component.scss',
})
export class WorkEvidenceComponent implements AfterViewInit {
  @ViewChild('workHoursChart') chartRef!: ElementRef;
  public chart: Chart | null = null;

  readonly authService = inject(AuthService);
  readonly shiftService = inject(ShiftApiService);
  private userService = inject(UserService);
  private organizationService = inject(OrganizationApiService);
  
  // View mode and date selection
  viewMode: 'month' | 'week' | 'custom' = 'month';
  selectedMonth: number = new Date().getMonth() + 1;
  selectedWeek: number = this.getCurrentWeek();
  selectedYear: number = new Date().getFullYear();
  startDate: string = '';
  endDate: string = '';
  selectedOrganizationId: string | null = null;
  organizations: Organization[] = [];
  organizationMembers: { uuid: string, name: string }[] = [];

  // Search functionality
  searchQuery = '';
  showAllEmployees = true;

  // Month and week options
  months = [
    { value: 1, label: 'Styczeń' },
    { value: 2, label: 'Luty' },
    { value: 3, label: 'Marzec' },
    { value: 4, label: 'Kwiecień' },
    { value: 5, label: 'Maj' },
    { value: 6, label: 'Czerwiec' },
    { value: 7, label: 'Lipiec' },
    { value: 8, label: 'Sierpień' },
    { value: 9, label: 'Wrzesień' },
    { value: 10, label: 'Październik' },
    { value: 11, label: 'Listopad' },
    { value: 12, label: 'Grudzień' },
  ];

  weeks = Array.from({ length: 52 }, (_, i) => ({
    value: i + 1,
    label: `Tydzień ${i + 1}`,
  }));

  testShift: Shift | null = null;
  error: string | null = null;
  //tabela pracowników do testowania
  employees: Employee[] = [];

  sortDirection = 'asc';
  sortedEmployees = [...this.employees];

  ngOnInit() {
  this.userService.getUserOrganizations().subscribe({
    next: (response) => {
      this.organizations = response.items;
      if (this.organizations.length > 0) {
        this.selectedOrganizationId = this.organizations[0].id;
        this.onOrganizationChange(); // automatycznie pobierz członków pierwszej organizacji
      }
    },
    error: (err) => {
      this.error = 'Nie udało się pobrać organizacji użytkownika.';
      console.error(err);
    }
  });
  }

  onOrganizationChange() {
    const selectedOrg = this.organizations.find(org => org.id === this.selectedOrganizationId);
    if (!selectedOrg) {
      this.employees = [];
      this.sortedEmployees = [];
      return;
    }
    console.log('Selected organization:', selectedOrg);
    // Pobierz członków organizacji przez API
    this.organizationService.getMembers(
      selectedOrg.id,
      {},
      { pageNumber: 1, pageSize: 100, sortField: 'fullName', ascending: true }
    ).subscribe({
      next: (response) => {
        console.log('Pobrano członków organizacji response:', response.items);
        // response.items to tablica Member
        this.employees = response.items.map(member => ({
          id: member.userId,
          name: member.firstName + ' ' + member.lastName,
          workEntries: [],
          showDetails: false
        }));
        console.log('Mapped employees:', this.employees);
        this.sortedEmployees = [...this.employees];
        this.loadWorkEntriesForEmployees();
      },
      error: (err) => {
        this.employees = [];
        this.sortedEmployees = [];
        this.error = 'Nie udało się pobrać członków organizacji.';
        console.error(err);
      }
    });
  }

  getCurrentWeek(): number {
    const now = new Date();
    const start = new Date(now.getFullYear(), 0, 1);
    const diff = now.getTime() - start.getTime();
    const oneWeek = 7 * 24 * 60 * 60 * 1000;
    return Math.ceil(diff / oneWeek);
  }

  getFilteredEntries(entries: WorkEntry[], start: Date, end: Date): WorkEntry[] {
    return entries.filter(entry => {
      const entryDate = new Date(entry.date);
      return entryDate >= start && entryDate <= end;
    });
  }

  getEntryDuration(entry: WorkEntry): number {
    const [startHours, startMinutes] = entry.startTime.split(':').map(Number);
    const [endHours, endMinutes] = entry.endTime.split(':').map(Number);

    const startTotalMinutes = startHours * 60 + startMinutes;
    const endTotalMinutes = endHours * 60 + endMinutes;

    return endTotalMinutes - startTotalMinutes;
  }

  getTotalHours(entries: WorkEntry[]): number {
    return entries.reduce((sum, entry) => sum + this.getEntryDuration(entry), 0) / 60;
  }

  getWeekGroups(entries: WorkEntry[]): WeekGroup[] {
    const weekGroups: WeekGroup[] = [];
    const entriesByWeek = new Map<number, WorkEntry[]>();

    entries.forEach(entry => {
      const date = new Date(entry.date);
      const weekNumber = this.getWeekNumber(date);

      if (!entriesByWeek.has(weekNumber)) {
        entriesByWeek.set(weekNumber, []);
      }
      entriesByWeek.get(weekNumber)?.push(entry);
    });

    entriesByWeek.forEach((weekEntries, weekNumber) => {
      const startDate = this.getDateFromWeek(weekNumber, this.selectedYear);
      const endDate = new Date(startDate);
      endDate.setDate(endDate.getDate() + 6);

      weekGroups.push({
        weekNumber,
        startDate,
        endDate,
        entries: weekEntries,
        isExpanded: false,
      });
    });

    return weekGroups.sort((a, b) => a.weekNumber - b.weekNumber);
  }

  getWeekNumber(date: Date): number {
    const start = new Date(date.getFullYear(), 0, 1);
    const diff = date.getTime() - start.getTime();
    const oneWeek = 7 * 24 * 60 * 60 * 1000;
    return Math.ceil(diff / oneWeek);
  }

  getWeekHours(weekGroup: WeekGroup): number {
    return this.getTotalHours(weekGroup.entries);
  }

  getDayHours(entries: WorkEntry[], date: string): number {
    const dayEntry = entries.find(entry => entry.date === date);
    return dayEntry ? this.getEntryDuration(dayEntry) / 60 : 0;
  }

  getWeekEntries(entries: WorkEntry[], weekStart: Date, weekEnd: Date): WorkEntry[] {
    return this.getFilteredEntries(entries, weekStart, weekEnd);
  }

  loadData() {
    this.error = null;

    if (!this.authService.hasValidAccessToken()) {
      this.error = 'Nie jesteś zalogowany. Zaloguj się najpierw.';
      this.authService.initLoginFlow();
      return;
    }

    let start: Date;
    let end: Date;

    switch (this.viewMode) {
      case 'month':
        start = new Date(this.selectedYear, this.selectedMonth - 1, 1);
        end = new Date(this.selectedYear, this.selectedMonth, 0);
        break;
      case 'week':
        start = this.getDateFromWeek(this.selectedWeek, this.selectedYear);
        end = new Date(start);
        end.setDate(end.getDate() + 6);
        break;
      case 'custom':
        start = new Date(this.startDate);
        end = new Date(this.endDate);
        break;
      default:
        start = new Date();
        end = new Date();
    }

    // Aktualizacja wykresu dla aktualnie wybranego pracownika
    const selectedEmployee = this.employees.find(emp => emp.showDetails);
    if (selectedEmployee) {
      setTimeout(() => {
        this.updateChartData(selectedEmployee);
      }, 0);
    }

    // TODO: Implement API call to get work entries for the selected period
    console.log('Loading data for period:', { start, end });
  }

  private getDateFromWeek(week: number, year: number): Date {
    const date = new Date(year, 0, 1);
    const dayOffset = (week - 1) * 7;
    date.setDate(date.getDate() + dayOffset);
    return date;
  }


  sortEmployees() {
    this.sortedEmployees = [...this.employees].sort((a, b) => {
      const aHours = this.getTotalHours(this.getCurrentPeriodEntries(a));
      const bHours = this.getTotalHours(this.getCurrentPeriodEntries(b));

      if (this.sortDirection === 'asc') {
        return aHours - bHours;
      } else {
        return bHours - aHours;
      }
    });
  }

  toggleSort() {
    this.sortDirection = this.sortDirection === 'asc' ? 'desc' : 'asc';
    this.sortEmployees();
  }

  toggleDetails(emp: Employee) {
    console.log('Toggling details for employee:', emp.name);
    emp.showDetails = !emp.showDetails;
    if (emp.showDetails) {
      const currentEntries = this.getCurrentPeriodEntries(emp);
      emp.weekGroups = this.getWeekGroups(currentEntries);
      // Wait for the next tick to ensure the canvas is in the DOM
      requestAnimationFrame(() => {
        this.updateChartData(emp);
      });
    } else if (this.chart) {
      // Destroy chart when hiding details
      this.chart.destroy();
      this.chart = null;
    }
  }

  switchView() {
    this.viewMode = this.viewMode === 'month' ? 'week' : 'month';
    // Ponowna inicjalizacja wykresu dla aktualnie wybranego pracownika
    const selectedEmployee = this.employees.find(emp => emp.showDetails);
    if (selectedEmployee) {
      setTimeout(() => {
        this.updateChartData(selectedEmployee);
      }, 0);
    }
  }

  // Helper method to get current period entries
  getCurrentPeriodEntries(employee: Employee): WorkEntry[] {
    let start: Date;
    let end: Date;

    switch (this.viewMode) {
      case 'month':
        start = new Date(this.selectedYear, this.selectedMonth - 1, 1);
        end = new Date(this.selectedYear, this.selectedMonth, 0);
        break;
      case 'week':
        start = this.getDateFromWeek(this.selectedWeek, this.selectedYear);
        end = new Date(start);
        end.setDate(end.getDate() + 6);
        break;
      case 'custom':
        start = new Date(this.startDate);
        end = new Date(this.endDate);
        break;
      default:
        start = new Date();
        end = new Date();
    }

    return this.getFilteredEntries(employee.workEntries, start, end);
  }

  toggleWeekDetails(employee: Employee, weekGroup: WeekGroup) {
    weekGroup.isExpanded = !weekGroup.isExpanded;
  }

  ngAfterViewInit() {
    this.initializeChart();
  }

  private initializeChart() {
    console.log('Initializing chart...');
    if (this.chartRef?.nativeElement) {
      console.log('Chart canvas found');
      const ctx = this.chartRef.nativeElement.getContext('2d');

      // Destroy existing chart if it exists
      if (this.chart) {
        this.chart.destroy();
      }

      // Kolor słupków jak przycisk primary
      const primaryColor = 'rgb(68, 187, 164)'; // #44bba4

      this.chart = new Chart(ctx, {
        type: 'bar',
        data: {
          labels: [],
          datasets: [
            {
              label: 'Przepracowane godziny',
              data: [],
              backgroundColor: primaryColor,
              borderWidth: 2,
              borderRadius: 5,
              barThickness: 20,
              maxBarThickness: 30,
              minBarLength: 5,
            },
          ],
        },
        options: {
          responsive: true,
          maintainAspectRatio: false,
          animation: {
            duration: 1000,
            easing: 'easeInOutQuart',
          },
          scales: {
            x: {
              title: {
                display: true,
                text: 'Okres',
                color: '#fff',
                font: {
                  size: 14,
                  weight: 'bold',
                },
                padding: { top: 10 },
              },
              grid: {
                display: false,
              },
              ticks: {
                color: '#fff',
                font: {
                  size: 12,
                },
              },
            },
            y: {
              title: {
                display: true,
                text: 'Przepracowane godziny',
                color: '#fff',
                font: {
                  size: 14,
                  weight: 'bold',
                },
                padding: { bottom: 10 },
              },
              beginAtZero: true,
              grid: {
                color: 'rgba(255, 255, 255, 0.1)',
              },
              ticks: {
                color: '#fff',
                font: {
                  size: 12,
                },
                callback: function (value) {
                  return value + 'h';
                },
              },
            },
          },
          plugins: {
            legend: {
              display: true,
              position: 'top',
              labels: {
                color: '#fff',
                font: {
                  size: 14,
                  weight: 'bold',
                },
                padding: 20,
              },
            },
            tooltip: {
              backgroundColor: 'rgba(0, 0, 0, 0.8)',
              padding: 12,
              titleColor: '#fff',
              bodyColor: '#fff',
              titleFont: {
                size: 14,
                weight: 'bold',
              },
              bodyFont: {
                size: 13,
              },
              callbacks: {
                label: context => `${context.parsed.y.toFixed(1)} godzin`,
                title: context => {
                  const label = context[0].label;
                  return `Okres: ${label}`;
                },
              },
            },
          },
          onClick: (event, elements) => {
            if (elements && elements.length > 0) {
              const index = elements[0].index;
              const label = this.chart?.data.labels?.[index];

              if (this.chart?.data.datasets[0].label?.includes('Rozkład dni')) {
                // Jeśli jesteśmy w widoku szczegółowym, wróć do widoku tygodniowego
                this.resetToWeeklyView();
              } else if (label?.toString().startsWith('Tydzień')) {
                // Jeśli jesteśmy w widoku tygodniowym, pokaż szczegóły tygodnia
                const weekNumber = parseInt(label.toString().split(' ')[1]);
                this.showWeekDetails(weekNumber);
              }
            }
          },
        },
      });

      console.log('Chart initialized');
    } else {
      console.log('Chart canvas not found');
    }
  }

  private updateChartData(employee: Employee) {
    console.log('Updating chart data for employee:', employee.name);

    // Initialize chart if it doesn't exist
    if (!this.chart) {
      this.initializeChart();
    }

    if (!this.chart) {
      console.log('Failed to initialize chart');
      return;
    }

    const entries = this.getCurrentPeriodEntries(employee);
    console.log('Current period entries:', entries);
    let labels: string[] = [];
    let data: number[] = [];

    if (this.viewMode === 'month') {
      // Zawsze grupuj po tygodniach dla widoku miesięcznego
      const weekGroups = this.getWeekGroups(entries);
      console.log('Week groups:', weekGroups);
      labels = weekGroups.map(group => `Tydzień ${group.weekNumber}`);
      data = weekGroups.map(group => this.getWeekHours(group));
    } else if (this.viewMode === 'custom') {
      // Dla własnego zakresu sprawdź długość okresu
      const startDate = new Date(this.startDate);
      const endDate = new Date(this.endDate);
      const daysDiff = Math.ceil((endDate.getTime() - startDate.getTime()) / (1000 * 60 * 60 * 24));

      if (daysDiff <= 14) {
        // Jeśli okres jest krótszy niż 2 tygodnie
        // Pokaż dane dzienne
        const sortedEntries = [...entries].sort(
          (a, b) => new Date(a.date).getTime() - new Date(b.date).getTime(),
        );
        labels = sortedEntries.map(entry =>
          new Date(entry.date).toLocaleDateString('pl-PL', { weekday: 'short', day: 'numeric' }),
        );
        data = sortedEntries.map(entry => this.getEntryDuration(entry) / 60);
      } else {
        // Dla dłuższych okresów grupuj po tygodniach
        const weekGroups = this.getWeekGroups(entries);
        labels = weekGroups.map(group => `Tydzień ${group.weekNumber}`);
        data = weekGroups.map(group => this.getWeekHours(group));
      }
    } else {
      // Dla widoku tygodniowego zawsze pokazuj dane dzienne
      const sortedEntries = [...entries].sort(
        (a, b) => new Date(a.date).getTime() - new Date(b.date).getTime(),
      );
      labels = sortedEntries.map(entry =>
        new Date(entry.date).toLocaleDateString('pl-PL', { weekday: 'short', day: 'numeric' }),
      );
      data = sortedEntries.map(entry => this.getEntryDuration(entry) / 60);
    }

    console.log('Chart labels:', labels);
    console.log('Chart data:', data);

    this.chart.data.labels = labels;
    this.chart.data.datasets[0].data = data;
    this.chart.data.datasets[0].label = 'Przepracowane godziny';
    this.chart.update('none');
    console.log('Chart updated');
  }

  private showWeekDetails(weekNumber: number) {
    const selectedEmployee = this.employees.find(emp => emp.showDetails);
    if (!selectedEmployee) return;

    const entries = this.getCurrentPeriodEntries(selectedEmployee);
    const weekGroups = this.getWeekGroups(entries);
    const selectedWeek = weekGroups.find(group => group.weekNumber === weekNumber);

    if (selectedWeek) {
      // Sortuj wpisy po dacie
      const sortedEntries = [...selectedWeek.entries].sort((a, b) => 
        new Date(a.date).getTime() - new Date(b.date).getTime()
      );

      // Przygotuj dane do wykresu
      const labels = sortedEntries.map(entry => 
        new Date(entry.date).toLocaleDateString('pl-PL', { weekday: 'short', day: 'numeric' })
      );
      const data = sortedEntries.map(entry => this.getEntryDuration(entry) / 60);

      if (this.chart) {
        this.chart.data.labels = labels;
        this.chart.data.datasets[0].data = data;
        this.chart.data.datasets[0].label = `Tydzień ${weekNumber} - Rozkład dni`;
        this.chart.update('none'); // Wyłącz animację przy aktualizacji
      }
    }
  }

  public resetToWeeklyView() {
    const selectedEmployee = this.employees.find(emp => emp.showDetails);
    if (selectedEmployee) {
      // Zniszcz i zainicjalizuj wykres na nowo
      if (this.chart) {
        this.chart.destroy();
        this.chart = null;
      }
      this.initializeChart();
      this.updateChartData(selectedEmployee);
    }
  }

  get filteredEmployees(): Employee[] {
    if (this.showAllEmployees) {
      return this.sortedEmployees;
    }

    if (!this.searchQuery.trim()) {
      return this.sortedEmployees;
    }

    const query = this.searchQuery.toLowerCase().trim();
    return this.sortedEmployees.filter(emp => emp.name.toLowerCase().includes(query));
  }

  toggleShowAll() {
    this.showAllEmployees = !this.showAllEmployees;
    this.searchQuery = ''; // Reset search when switching modes
  }

  onSearchChange() {
    // Automatycznie przełącz na tryb wyszukiwania gdy użytkownik zacznie wpisywać
    if (this.searchQuery.trim() && this.showAllEmployees) {
      this.showAllEmployees = false;
    }
  }

  // Dodaj metodę do pobierania wszystkich pracowników
  loadWorkEntriesForEmployees() {
    this.error = null;
    if (!this.employees || this.employees.length === 0) return;
    console.log('Loading work entries for employees:', this.employees);
    this.employees.forEach(employee => {
      console.log(`Loading work entries for employee: ${employee.name} (ID: ${employee.id})`);
      this.shiftService.getAllShiftsForMember(employee.id).subscribe({
        next: (shifts: Shift[]) => {
          employee.workEntries = shifts.map(shift => ({
            date: new Date(shift.startTime).toISOString().split('T')[0],
            startTime: new Date(shift.startTime).toTimeString().substring(0, 5),
            endTime: new Date(shift.endTime).toTimeString().substring(0, 5)
          }));
          this.sortedEmployees = [...this.employees];
        },
        error: (err) => {
          console.error(`Błąd podczas pobierania wpisów pracy dla pracownika ${employee.name}:`, err);
        }
      });
    });
    console.log('Finished initiating load for all employees', this.employees);
  }
}
