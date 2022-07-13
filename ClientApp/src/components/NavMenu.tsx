import * as React from 'react';
import { Collapse, Container, Nav, Navbar, NavbarBrand, NavbarToggler, NavItem, NavLink } from 'reactstrap';
import { Link } from 'react-router-dom';
import { ApplicationState } from '../store';
import * as TaskStore from '../store/Tasks';
import { connect } from 'react-redux';
import './NavMenu.css';

type TaskProps =
    TaskStore.TasksState
    & typeof TaskStore.actionCreators;

class NavMenu extends React.PureComponent<TaskProps> {
    constructor(props: any) {
        super(props);

        this.toggleNavbar = this.toggleNavbar.bind(this);
        this.state = {
            collapsed: true
        };
    }
    public state = {
        collapsed: false,
    };

    public componentDidMount() {
        this.ensureDataFetched();
    }

    private ensureDataFetched() {
        this.props.requestUsers();
    }

    toggleNavbar() {
        this.setState({
            collapsed: !this.state.collapsed
        });
    }

    public render() {
        return (
            <header>
                {/*<Navbar color="faded" light>*/}
                <Navbar className="navbar-expand-sm navbar-toggleable-sm border-bottom box-shadow mb-3" light>
                    <Container>
                        <NavbarBrand tag={Link} to="/">Todos</NavbarBrand>
                        <NavbarToggler onClick={this.toggleNavbar} className="mr-2" />
                        <Collapse isOpen={!this.state.collapsed} navbar>
                            <Nav navbar>
                                {this.props.AllUsers.map(user =>
                                    <NavItem>
                                        <NavLink key={`/${user.userId}`} tag={Link} to={`/${user.userId}`} onClick={() => this.props.requestUser(user.userId)}> {user.userName}</NavLink>
                                    </NavItem>
                                )}
                            </Nav>
                        </Collapse>
                    </Container>
                </Navbar>
            </header>
        );
    }
}

export default connect(
    (state: ApplicationState) => state.tasks, // Selects which state properties are merged into the component's props
    TaskStore.actionCreators // Selects which action creators are merged into the component's props
)(NavMenu as any); // eslint-disable-line @typescript-eslint/no-explicit-any

