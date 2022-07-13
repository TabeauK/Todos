import * as React from 'react';
import { connect } from 'react-redux';
/*import { Link } from 'react-router-dom';*/
import { ApplicationState } from '../store';
import * as TaskStore from '../store/Tasks';
import { DataView, DataViewLayoutOptions } from 'primereact/dataview';
import { Button } from 'primereact/button';
import { OverlayPanel } from 'primereact/overlaypanel';
import './Home.css';
import 'primeflex/primeflex.css';
import { InputText } from 'primereact/inputtext';
import { Calendar } from 'primereact/calendar';
import { InputNumber } from 'primereact/inputnumber';   


// At runtime, Redux will merge together...
type TaskProps =
    TaskStore.TasksState
    & typeof TaskStore.actionCreators;

class FetchData extends React.PureComponent<TaskProps> {
    public state = {
        layout: "list",
        op: React.createRef<OverlayPanel>(),
        newTaskName: "",
        newTaskNameValid: true,
        newTaskTimespan: 30,
        newTaskTimespanValid: true,
        newTaskLastDate: new Date(),
        newTaskLastDateValid: true,
        scheduleOp: React.createRef<OverlayPanel>(),
        scheduleDate: new Date(),
        scheduleTime: new Date("2020-01-01 01:00"),
        scheduleDateValid: true,
        scheduleTimeValid: true,
    };

    public componentDidMount() {
        this.props.requestUser(null);
        setInterval(() => this.props.requestTasks() , 3000)
    }

    public render() {
        return (
            <React.Fragment>
                <div>
                    <h3 id="tabelLabel">
                        {this.props.User ? this.props.User.userName : ""}
                    </h3>
                    <div>
                        {this.renderTable()}
                    </div>
                </div>
            </React.Fragment>
        );
    }

    private validateNewTaskName() {
        if (this.state.newTaskName.length < 1)  
            this.setState({ newTaskNameValid: false })
        else
            this.setState({ newTaskNameValid: true })
        return this.state.newTaskNameValid;
    }

    private validateNewTaskLastDate() {
        if (this.state.newTaskLastDate === null)
            this.setState({ newTaskLastDateValid: true })
        else if (this.state.newTaskLastDate > new Date(Date.now()))
            this.setState({ newTaskLastDateValid: false })
        else 
            this.setState({ newTaskLastDateValid: true })
        return this.state.newTaskLastDate;
    }

    private validateNewTaskTimespan() {
        if (this.state.newTaskTimespan === null || this.state.newTaskTimespan < 1)
            this.setState({ newTaskTimespanValid: false })
        else
            this.setState({ newTaskTimespanValid: true })
        return this.state.newTaskTimespanValid;
    }

    private validateScheduleDate() {
        if (this.state.scheduleDate === null || this.state.scheduleDate < new Date(Date.now()))
            this.setState({ scheduleDateValid: false })
        else
            this.setState({ scheduleDateValid: true })
        return this.state.scheduleDateValid;
    }

    private validateScheduleTime() {
        this.setState({ scheduleTimeValid: true })
        return this.state.scheduleTimeValid;
    }

    private renderTable() {
        return(
            <div className="dataview-demo" >
                <div className="card">
                    <DataView value={this.props.Tasks} layout={this.state.layout} header={this.renderHeader()}
                        itemTemplate={this.itemTemplate} paginator rows={9} emptyMessage={"Brak zadań"} sortOrder={-1} sortField={"state"}/>
                </div>
            </div>
        );
    }

    private deadline = (task: TaskStore.Task) => {
        let nextDate = new Date(task.addedDate);
        if (task.checks !== [] && task.checks !== null && task.checks !== undefined)
            nextDate = new Date(task.checks[task.checks.length - 1].date);
        nextDate.setDate(nextDate.getDate() + task.interval + task.extraTime)
        return nextDate;
    }

    private statusName = (task: TaskStore.Task) => {
        switch (task.state) {
            case 1:
                return "Zadanie zaplanowane";
            case 2:
                return "Zadanie bezpiecznie daleko";
            case 4:
                return "Do zrealizowania ASAP";
            case 5:
                return "Zadanie się zbiliża";
            case 6:
                return "Zaplanuj zadanie";
            default:
                return "Coś poszło nie tak";
        }
    }

    private statusColor = (task: TaskStore.Task) => {
        switch (task.state) {
            case 1:
                return { borderColor: "blue"};
            case 2:
                return { borderColor: "green" };
            case 4:
                return { borderColor: "red" };
            case 5:
                return { borderColor: "yellow" };
            case 6:
                return { borderColor: "orange" };
            default:
                return { borderColor: "black" };;
        }
    }


    private renderHeader = () => {
        return (
            <div className="p-grid p-nogutter">
                <div className="p-col-6" style={{ textAlign: 'left' }}>
                    <Button type="button" icon="pi pi-plus" label={'Dodaj'} onClick={(e) => (this.state.op.current as OverlayPanel).toggle(e)} aria-haspopup aria-controls="overlay_panel" className="select-product-button" />
                    <OverlayPanel ref={this.state.op} showCloseIcon id="overlay_panel" style={{ width: '450px', padding:'10px'}} className="overlaypanel-demo">
                        <div className="p-float-label p-mb-4 p-fluid">
                            <InputText
                                id="in"
                                value={this.state.newTaskName}
                                onChange={(e) => this.setState({ newTaskName: e.target.value }, () => this.validateNewTaskName())}
                                className={this.state.newTaskNameValid ? "" : "p-invalid"}
                            />
                            <label htmlFor="in">Nazwa</label>
                        </div>
                        <div className="p-float-label p-mb-4 p-fluid">
                            <div>Ostatnie zaliczenie zadania</div>
                            <Calendar
                                id="basic"
                                value={this.state.newTaskLastDate === null ? this.state.newTaskLastDate : undefined}
                                onChange={(e) => this.setState({ newTaskLastDate: e.target.value }, () => this.validateNewTaskLastDate())}
                                maxDate={new Date(Date.now())}
                                className={"p-d-flex p-float-label " + this.state.newTaskLastDateValid ? "" : "p-invalid"}
                            />
                        </div>
                        <div className="p-float-label p-mb-4 p-fluid">
                            <div>Cykl w dniach</div>
                            <InputNumber
                                value={this.state.newTaskTimespan}
                                onChange={(e) => this.setState({ newTaskTimespan: e.value }, () => this.validateNewTaskTimespan())}
                                showButtons
                                min={1}
                                className={this.state.newTaskTimespanValid ? "" : "p-invalid"}
                            />
                        </div>
                        <div className="p-float-label p-mb-2 p-fluid">
                            <Button type="button" icon="pi pi-plus" label={'Dodaj'} onClick={(e) => {
                                this.validateNewTaskName();
                                this.validateNewTaskLastDate();
                                this.validateNewTaskTimespan();
                                if (this.state.newTaskLastDateValid &&
                                    this.state.newTaskNameValid &&
                                    this.state.newTaskTimespanValid) {
                                    this.props.addTaskAction(
                                        this.state.newTaskName,
                                        this.state.newTaskTimespan,
                                        this.state.newTaskLastDate
                                    );
                                    this.props.requestTasks();
                                    this.setState({
                                        newTaskName: "",
                                        newTaskNameValid: true,
                                        newTaskTimespan: 30,
                                        newTaskTimespanValid: true,
                                        newTaskLastDate: new Date(),
                                        newTaskLastDateValid: true,
                                    });
                                    (this.state.op.current as OverlayPanel).hide();
                                }}
                            } />
                        </div>
                    </ OverlayPanel>
                </div>
                <div className="p-col-6" style={{ textAlign: 'right' }}>
                    <DataViewLayoutOptions layout={this.state.layout} onChange={(e) => this.setState({ layout: e.value })} />
                </div>
            </div>
        );
    }

    private ScheduleOverlay = (data: TaskStore.Task) => {
        return ( 
            <OverlayPanel ref={this.state.scheduleOp} showCloseIcon id="overlay_panel" style={{ width: '450px', padding:'10px'}} className="overlaypanel-demo">

                <div className="p-float-label p-mb-4 p-fluid">
                    <label htmlFor="scheduleStart">Data i godzina rozpoczęcia</label>
                    <Calendar
                        id="scheduleStart"
                        value={this.state.scheduleDate}
                        onChange={(e) => this.setState({ scheduleDate: e.target.value }, () => this.validateScheduleDate())}
                        minDate={new Date(Date.now())}
                        showTime
                        hourFormat="24"
                        className={"p-d-flex p-float-label " + this.state.scheduleDateValid ? "" : "p-invalid"}
                    />
                </div>
                <div className="p-float-label p-mb-4 p-fluid">
                    <label htmlFor="scheduleTime">Czas trwania</label>
                    <Calendar
                        id="scheduleTime"
                        value={this.state.scheduleTime}
                        onChange={(e) => this.setState({ scheduleTime: e.target.value }, () => this.validateScheduleTime())}
                        timeOnly
                        hourFormat="24"
                        className={"p-d-flex p-float-label " + this.state.scheduleTimeValid ? "" : "p-invalid"}
                    />
                </div>
                <div className="p-float-label p-mb-2 p-fluid">
                    <Button type="button" icon="pi pi-calendar-plus" label={'Zaplanuj'} onClick={(e) => {
                        this.validateScheduleDate();
                        this.validateScheduleTime();
                        if (this.state.scheduleDateValid &&
                            this.state.scheduleTimeValid) {
                            this.props.scheduleTaskAction(
                                data,
                                this.state.scheduleDate,
                                this.state.scheduleTime
                            );
                            this.props.requestTasks();
                            this.setState({
                                scheduleDate: new Date(),
                                scheduleTime: new Date("2020-01-01 01:00"),
                                scheduleDateValid: true,
                                scheduleTimeValid: true,
                            });
                            (this.state.scheduleOp.current as OverlayPanel).hide();
                        }}
                    } />
                </div>
            </ OverlayPanel>);
    }

    private renderListItem = (data: TaskStore.Task) => {
        let nextDate = new Date(data.addedDate);
        if (data.checks !== [] && data.checks !== null && data.checks !== undefined)
            nextDate = new Date(data.checks[data.checks.length - 1].date);
        return (
            <div className="p-col-12">
                <div className="product-list-item" style={this.statusColor(data)}>
                    <div className="product-list-detail">
                        <div className="product-name">{data.name}</div>
                        <div className="product-description">
                            <div>Ostatnie wykonanie: {nextDate.toLocaleDateString("pl-PL")}</div>
                            <div>Deadline: {this.deadline(data).toLocaleDateString("pl-PL")}</div>
                            <div>Cykl: {data.interval} dni</div>
                        </div>
                        <i className="pi pi-tag product-category-icon"></i><span className="product-category">{this.statusName(data)}</span>
                    </div>
                    <div className="product-list-action">
                        <Button type="button" icon="pi pi-calendar" label={'Zaplanuj'} onClick={(e) => (this.state.scheduleOp.current as OverlayPanel).toggle(e)} aria-haspopup aria-controls="overlay_panel" className="select-product-button" />
                        {this.ScheduleOverlay(data)}
                        <Button icon="pi pi-check-square" label="Zrobione" onClick={() => this.props.checkTaskAction(data)} />
                        <Button icon="pi pi-step-forward" label="Odłóż" onClick={() => this.props.postponeTaskAction(data)} />
                        <Button icon="pi pi-trash" label="Usuń" onClick={() => this.props.deleteTaskAction(data)} />
                    </div>
                </div>
            </div>
        );
    }

    private renderGridItem = (data: TaskStore.Task) => {
        let nextDate = new Date(data.addedDate);
        if (data.checks !== [] && data.checks !== null && data.checks !== undefined)
            nextDate = new Date(data.checks[data.checks.length - 1].date);
        return (
            <div className="p-col-12 p-md-4">
                <div className="product-grid-item card" style={this.statusColor(data)}>
                    <div className="product-grid-item-top">
                        <div>
                            <i className="pi pi-tag product-category-icon"></i><span className="product-category">{this.statusName(data)}</span>
                        </div>
                    </div>
                    <div className="product-grid-item-content">
                        <div className="product-name">{data.name}</div>
                        <div className="product-description">
                            <div>Ostatnie wykonanie: {nextDate.toLocaleDateString("pl-PL")}</div>
                            <div>Deadline: {this.deadline(data).toLocaleDateString("pl-PL")}</div>
                            <div>Cykl: {data.interval} dni</div>
                        </div>
                    </div>
                    <div className="product-grid-item-bottom">
                        <Button type="button" icon="pi pi-calendar" label={'Zaplanuj'} onClick={(e) => (this.state.scheduleOp.current as OverlayPanel).toggle(e)} aria-haspopup aria-controls="overlay_panel" className="select-product-button" />
                        {this.ScheduleOverlay(data)}
                        <Button icon="pi pi-check-square" label="Zrobione" onClick={() => this.props.checkTaskAction(data)} />
                        <Button icon="pi pi-step-forward" label="Odłóż" onClick={() => this.props.postponeTaskAction(data)} />
                        <Button icon="pi pi-trash" label="Usuń" onClick={() => this.props.deleteTaskAction(data)} />
                    </div>
                </div>
            </div>
        );
    }

    private itemTemplate = (product : TaskStore.Task, layout: string) => {
        if (!product) {
            return;
        }
        if (layout === 'list')
            return this.renderListItem(product);
        else if (layout === 'grid')
            return this.renderGridItem(product);
    }
}

export default connect(
    (state: ApplicationState) => state.tasks, // Selects which state properties are merged into the component's props
    TaskStore.actionCreators // Selects which action creators are merged into the component's props
)(FetchData as any); // eslint-disable-line @typescript-eslint/no-explicit-any
