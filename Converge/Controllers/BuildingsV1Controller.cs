﻿// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Converge.Models;
using Converge.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Converge.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/v1.0/buildings")]
    public class BuildingsV1Controller : Controller
    {
        /// <summary>
        /// Send logs to telemetry service
        /// </summary>
        private readonly BuildingsService buildingsService;

        public BuildingsV1Controller(BuildingsService buildingsService)
        {
            this.buildingsService = buildingsService;
        }

        /// <summary>
        /// Returns all the Buildings alone (not Places) registered in the System, supporting Pagination, 
        /// defaulted to first 100 buildings if there is no value on how many records to skip.
        /// </summary>
        /// <param name="topCount">Number of records after the skipCount used to skip the number of records.</param>
        /// <param name="skip">The number of records to skip.</param>
        /// <returns>BuildingsResponse: Containing the list of Buildings records and the count of those.</returns>
        [HttpGet]
        [Route("sortByName")]
        public async Task<ActionResult<BasicBuildingsResponse>> GetBuildingsByName(int? topCount = 10, int? skip = 0)
        {
            buildingsService.SetPrincipalUserIdentity(User.Identity);
            var result = await buildingsService.GetBuildingsByName(topCount, skip);
            return Ok(result);
        }

        /// <summary>
        /// Returns the Buildings alone (not Places) registered in the System, 
        /// to support fetching by distance from the source geo-coordinates provided.
        /// </summary>
        /// <param name="sourceGeoCoordinates">Comma-separated Latitude and Longitude representing Geo-coordinates</param>
        /// <param name="distanceFromSource">Distance in miles from the location mentioned by SourceGeoCoordinates parameter</param>
        /// <returns>BuildingsResponse: Containing the list of Buildings records and the count of those.</returns>
        [HttpGet]
        [Route("sortByDistance")]
        public async Task<ActionResult<BasicBuildingsResponse>> GetBuildingsByDistance(string sourceGeoCoordinates, double? distanceFromSource)
        {
            buildingsService.SetPrincipalUserIdentity(User.Identity);
            var result = await buildingsService.GetBuildingsByDistance(sourceGeoCoordinates, distanceFromSource);
            return Ok(result);
        }

        /// <summary>
        /// Get Building details by given display-name of Building.
        /// </summary>
        /// <param name="buildingDisplayName">Building-name</param>
        /// <returns>Building and its details</returns>
        [HttpGet]
        [Route("buildingByName/{buildingDisplayName}")]
        public async Task<ActionResult<BuildingBasicInfo>> GetBuildingByDisplayName(string buildingDisplayName)
        {
            var result = await buildingsService.GetBuildingByDisplayName(buildingDisplayName);
            return Ok(result);
        }

        /// <summary>
        /// Returns all the Conference-rooms belonging to the Building referenced using its UPN, supporting Pagination, 
        /// defaulted to first 100 Conference rooms if there is no value on how many records to skip.
        /// </summary>
        /// <param name="buildingUpn">UPN of Building.</param>
        /// <param name="topCount">Number of records after the skipCount used to skip the number of records.</param>
        /// <param name="skipToken">Skip-token option as string to get next set of records.</param>
        /// <param name="hasVideo">Whether to return places with a video display device.</param>
        /// <param name="hasAudio">Whether to return places with an audio device.</param>
        /// <param name="hasDisplay">Whether to return places with a display device.</param>
        /// <param name="isWheelchairAccessible">Whether to return places that are wheelchair accessible.</param>
        /// <param name="fullyEnclosed">Whether to return places that are fully enclosed</param>
        /// <param name="surfaceHub">Whether to return places with a surface hub</param>
        /// <param name="whiteboardCamera">Whether to return places with a whiteboard camera</param>
        /// <returns>ExchangePlacesResponse: Containing the Conference-rooms list and reference to Link-to-next-page.</returns>
        [HttpGet]
        [Route("{buildingUpn}/rooms")]
        public async Task<ActionResult<GraphExchangePlacesResponse>> GetBuildingConferenceRooms(string buildingUpn, 
                                                                                                int? topCount = null, 
                                                                                                string skipToken = null,
                                                                                                bool hasVideo = false,
                                                                                                bool hasAudio = false,
                                                                                                bool hasDisplay = false,
                                                                                                bool isWheelchairAccessible = false,
                                                                                                bool fullyEnclosed  = false,
                                                                                                bool surfaceHub = false,
                                                                                                bool whiteboardCamera = false)
        {
            ListItemFilterOptions listItemFilterOptions = new ListItemFilterOptions
            {
                HasAudio = hasAudio,
                HasVideo = hasVideo,
                HasDisplay = hasDisplay,
                IsWheelChairAccessible = isWheelchairAccessible,
                FullyEnclosed = fullyEnclosed,
                SurfaceHub = surfaceHub,
                WhiteboardCamera = whiteboardCamera,
            };
            var result = await buildingsService.GetPlacesOfBuilding(buildingUpn, PlaceType.Room, topCount, skipToken, listItemFilterOptions);
            return Ok(result);
        }

        /// <summary>
        /// Returns all the Workspaces belonging to the Building referenced using its UPN, supporting Pagination, 
        /// defaulted to first 100 Workspaces if there is no value on how many records to skip.
        /// </summary>
        /// <param name="buildingUpn">UPN of Building.</param>
        /// <param name="topCount">Number of records after the skipCount used to skip the number of records.</param>
        /// <param name="skipToken">Skip-token option as string to get next set of records.</param>
        /// <param name="hasVideo">Whether to return spaces with a video display device.</param>
        /// <param name="hasAudio">Whether to return spaces with an audio device.</param>
        /// <param name="hasDisplay">Whether to return spaces with a display device.</param>
        /// <param name="isWheelchairAccessible">Whether to return spaces that are wheelchair accessible.</param>
        /// <param name="displayNameSearchString">Search string to search spaces by display name</param>
        /// <param name="fullyEnclosed">Whether to return spaces that are fully enclosed</param>
        /// <param name="surfaceHub">Whether to return spaces with a surface hub</param>
        /// <param name="whiteboardCamera">Whether to return spaces with a whiteboard camera</param>
        /// <returns>ExchangePlacesResponse: Containing the Workspaces list and reference to Link-to-next-page.</returns>
        [HttpGet]
        [Route("{buildingUpn}/spaces")]
        public async Task<ActionResult<GraphExchangePlacesResponse>> GetBuildingWorkspaces(string buildingUpn,
                                                                                            int? topCount = null,
                                                                                            string skipToken = null,
                                                                                            bool hasVideo = false,
                                                                                            bool hasAudio = false,
                                                                                            bool hasDisplay = false,
                                                                                            bool isWheelchairAccessible = false,
                                                                                            string displayNameSearchString = null,
                                                                                            bool fullyEnclosed  = false,
                                                                                            bool surfaceHub = false,
                                                                                            bool whiteboardCamera = false)
        {
            ListItemFilterOptions listItemFilterOptions = new ListItemFilterOptions
            {
                HasAudio = hasAudio,
                HasVideo = hasVideo,
                HasDisplay = hasDisplay,
                IsWheelChairAccessible = isWheelchairAccessible,
                DisplayNameSearchString = displayNameSearchString,
                FullyEnclosed = fullyEnclosed,
                SurfaceHub = surfaceHub,
                WhiteboardCamera = whiteboardCamera,
            };
            var result = await buildingsService.GetPlacesOfBuilding(buildingUpn, PlaceType.Space, topCount, skipToken, listItemFilterOptions);
            return Ok(result);
        }

        /// <summary>
        /// Returns the Conference-room record for the given identity (upn).
        /// </summary>
        /// <param name="roomUpn">UPN of Conference-room.</param>
        /// <returns>ExchangePlace: Conference-room specifics.</returns>
        [HttpGet]
        [Route("rooms/{roomUpn}")]
        public async Task<ActionResult<ExchangePlace>> GetConferenceRoom(string roomUpn)
        {
            var result = await buildingsService.GetPlaceByUpn(roomUpn, PlaceType.Room);
            return Ok(result);
        }

        /// <summary>
        /// Returns the Workspace record for the given identity (upn).
        /// </summary>
        /// <param name="spaceUpn">UPN of Workspace.</param>
        /// <returns>ExchangePlace: Workspace specifics.</returns>
        [HttpGet]
        [Route("spaces/{spaceUpn}")]
        public async Task<ActionResult<ExchangePlace>> GetWorkspace(string spaceUpn)
        {
            var result = await buildingsService.GetPlaceByUpn(spaceUpn, PlaceType.Space);
            return Ok(result);
        }

        /// <summary>
        /// Returns in percentage (%) the Reserved and Availability of all the Workspaces for the given Building UPN.
        /// </summary>
        /// <param name="buildingUpn">UPN of Building.</param>
        /// <param name="start">From Date/time.</param>
        /// <param name="end">To Date/time.</param>
        /// <returns>ConvergeSchedule: Holds Reserved/Availability as Percentages (%) for the Workspaces.</returns>
        [HttpGet]
        [Route("{buildingUpn}/schedule")]
        public async Task<ActionResult<ConvergeSchedule>> GetWorkspacesSchedule(string buildingUpn, string start, string end)
        {
            return await buildingsService.GetWorkspacesScheduleForBuilding(buildingUpn, start, end);
        }

        [HttpGet]
        [Route("searchForBuildings/{searchString}")]
        public async Task<ActionResult<BuildingSearchInfo>> SearchForBuildings(string searchString, int? topCount = 10, int? skip = 0)
        {
            return await buildingsService.SearchForBuildings(searchString, topCount, skip);
        }
    }
}
