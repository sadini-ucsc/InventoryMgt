import {
  ChangeDetectionStrategy,
  Component,
  EventEmitter,
  Input,
  Output,
} from "@angular/core";
import { MatTableModule } from "@angular/material/table";
import { StockDisplayModel } from "../stock-display.model";
import { MatSortModule, Sort, SortDirection } from "@angular/material/sort";
import { MatIconModule } from "@angular/material/icon";

@Component({
    selector: "app-stock-list",
    imports: [MatTableModule, MatSortModule, MatIconModule],
    styles: [`
      .low-stock-row {
        background-color: #fff4e5;
      }
      .low-stock-warning {
        display: inline-flex;
        align-items: center;
        gap: 4px;
        margin-left: 12px;
        color: #b45309;
        font-weight: 500;
      }
      .low-stock-warning mat-icon {
        font-size: 18px;
        width: 18px;
        height: 18px;
      }
    `],
    changeDetection: ChangeDetectionStrategy.OnPush,
    template: `
    <table
      style=" margin-top: 1.5rem;"
      mat-table
      [dataSource]="stocks"
      matSort
      (matSortChange)="onSortData($event)"
      class="mat-elevation-z8"
    >
      <ng-container matColumnDef="productName">
        <th
          mat-header-cell
          *matHeaderCellDef
          mat-sort-header
          sortActionDescription="Sort by product name"
        >
          Product
        </th>
        <td mat-cell *matCellDef="let item">{{ item.productName }}</td>
      </ng-container>
      <ng-container matColumnDef="categoryName">
        <th
          mat-header-cell
          *matHeaderCellDef
          mat-sort-header
          sortActionDescription="sort by category"
        >
          Category
        </th>
        <td mat-cell *matCellDef="let item">{{ item.categoryName }}</td>
      </ng-container>

      <ng-container matColumnDef="quantity">
        <th mat-header-cell *matHeaderCellDef>Quantity</th>
        <td mat-cell *matCellDef="let item">
          {{ item.quantity }}
          @if(item.lowStock){
          <span class="low-stock-warning">
            <mat-icon aria-hidden="true">warning</mat-icon>
            Low stock
          </span>
          }
        </td>
      </ng-container>

      <tr mat-header-row *matHeaderRowDef="displayedColumns"></tr>
      <tr
        mat-row
        *matRowDef="let row; columns: displayedColumns"
        [class.low-stock-row]="row.lowStock"
      ></tr>
    </table>
  `
})
export class StockListComponent {
  @Input({ required: true }) stocks!: StockDisplayModel[];
  @Output() sort = new EventEmitter<{
    sortColumn: string;
    sortDirection: "asc" | "desc";
  }>();
  displayedColumns = ["productName", "categoryName", "quantity"];

  onSortData(sortData: Sort) {
    this.sort.emit({
      sortColumn: sortData.active,
      sortDirection: sortData.direction as "asc" | "desc",
    });
  }
}
