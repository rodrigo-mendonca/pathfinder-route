import React from 'react'
import {sleep } from '../html5'
import { withGoogleMap,GoogleMap, DirectionsRenderer,withScriptjs,Marker } from "react-google-maps"
/* eslint-disable no-undef */

class MapBase extends React.Component {

    constructor(props)
    {
        super(props)
        this.state = { center: {lat:0, lng:0}, directions:[]}
    }

    async componentDidMount(){
        if(!this.props.listEntregador)
            return

        const directions = []
        const routes = this.props.listEntregador

        if (!routes[0] || !routes[0].rotas[0])
            return

        for (const route of routes)
        {
            const mapedRoutes = route.rotas.map(e => ({
                                        from: { lat: e.saida.lat, lng: e.saida.lng },
                                        to: { lat: e.chegada.lat, lng: e.chegada.lng }
                                    }))
            const truck = []
     
            for (const d of mapedRoutes) {
                try {
                    const googleRoute = await this.GetGoogleRoute(d)
                    truck.push(googleRoute)
                } catch (error) {
                    console.warn(error)
                    await sleep(3000)
                    const googleRoute = await this.GetGoogleRoute(d)
                    truck.push(googleRoute)
                }
            }
            
            directions.push(truck)
        }
        const center = routes[this.props.currentIndex].rotas[0].saida
        this.setState({directions, center})
    }

    GetGoogleRoute = (points) => new Promise((resolve, reject) => {
        const DirectionsService = new google.maps.DirectionsService()

        DirectionsService.route({
            origin: new google.maps.LatLng(points.from.lat, points.from.lng),
            destination: new google.maps.LatLng(points.to.lat, points.to.lng),
            travelMode: google.maps.TravelMode.DRIVING,
        }, (result, status) => { if (status === google.maps.DirectionsStatus.OK) { resolve(result) } else { reject({status, result})} });
    })

    render() {
        const center = this.props.center || this.state.center
        const directions = this.state.directions ? this.state.directions[this.props.currentIndex] : undefined

        return (<GoogleMap
                defaultZoom={this.props.zoom}
                center={new google.maps.LatLng(center.lat, center.lng)} > 
                  {directions && directions.map((d,i) => <DirectionsRenderer key={i} directions={d} />)}

                  {this.props.markers && this.props.markers.map((m,i) => <Marker key={i} position={m}/> )}

               </GoogleMap>)
           }
}

const Map = withScriptjs(withGoogleMap(MapBase))
export default Map