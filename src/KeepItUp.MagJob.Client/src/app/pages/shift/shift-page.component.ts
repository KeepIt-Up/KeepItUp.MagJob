import { Component } from "@angular/core";
import { ShiftComponent } from "../../features/shift/shift.component";
import { CommonModule } from "@angular/common";
@Component({
  selector: "app-shift-page",
  templateUrl: "./shift-page.component.html",
  styleUrls: ["./shift-page.component.scss"],
  standalone: true,
  imports: [CommonModule, ShiftComponent]
})
export class ShiftPageComponent {

}